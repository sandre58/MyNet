// -----------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MyNet.Reflection;

/// <summary>
/// Helper class to work with types, for example, to get the type name without version information or to get the inner types of a generic type.
/// </summary>
public static class TypeHelper
{
    private const char InnerTypeCountStart = '`';
    private const char InternalTypeStart = '+';
    private const char InternalTypeEnd = '[';
    private const string AllTypesStart = "[[";
    private const char SingleTypeStart = '[';
    private const char SingleTypeEnd = ']';
    private static readonly char[] InnerTypeCountEnd = ['[', '+'];

    /// <summary>
    /// A list of microsoft public key tokens.
    /// </summary>
    private static readonly HashSet<string> MicrosoftPublicKeyTokens =
    [
        "b77a5c561934e089",
        "b03f5f7f11d50a3a",
        "31bf3856ad364e35"
    ];

    /// <summary>
    /// Gets the assembly by its name.
    /// </summary>
    /// <param name="name">The name of the assembly.</param>
    /// <returns>The assembly if found; otherwise, <c>null</c>.</returns>
    public static Assembly? GetAssemblyByName(string name) => AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);

    /// <summary>
    /// Gets the name of the assembly.
    /// </summary>
    /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
    /// <returns>The assembly name retrieved from the type, for example <c>Catel.Core</c> or <c>null</c> if the assembly is not contained by the type.</returns>
    /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
    public static string GetAssemblyName(string fullTypeName)
    {
        var lastGenericIndex = fullTypeName.LastIndexOf("]]", StringComparison.Ordinal);
        if (lastGenericIndex != -1)
        {
            fullTypeName = fullTypeName[(lastGenericIndex + 2)..];
        }

        var splitterPos = fullTypeName.IndexOf(", ", StringComparison.Ordinal);
        var assemblyName = splitterPos != -1 ? fullTypeName[(splitterPos + 1)..].Trim() : string.Empty;
        return assemblyName;
    }

    /// <summary>
    /// Gets the assembly name without overhead (version, public keytoken, etc).
    /// </summary>
    /// <param name="fullyQualifiedAssemblyName">Name of the fully qualified assembly.</param>
    /// <returns>The assembly without the overhead.</returns>
    /// <exception cref="ArgumentException">The <paramref name="fullyQualifiedAssemblyName"/> is <c>null</c> or whitespace.</exception>
    public static string GetAssemblyNameWithoutOverhead(string fullyQualifiedAssemblyName)
    {
        var indexOfFirstComma = fullyQualifiedAssemblyName.IndexOf(',', StringComparison.OrdinalIgnoreCase);
        return indexOfFirstComma != -1 ? fullyQualifiedAssemblyName[..indexOfFirstComma] : fullyQualifiedAssemblyName;
    }

    /// <summary>
    /// Gets the type name with assembly, but without the fully qualified assembly name. For example, this method provides
    /// the string:
    /// <para />
    /// <c>Catel.TypeHelper, Catel.Core, Version=1.0.0.0, PublicKeyToken=123456789</c>.
    /// <para />
    /// and will return:
    /// <para />
    /// <c>Catel.TypeHelper, Catel.Core</c>.
    /// </summary>
    /// <param name="fullTypeName">Full name of the type.</param>
    /// <returns>The type name including the assembly.</returns>
    /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
    public static string GetTypeNameWithAssembly(string fullTypeName)
    {
        var assemblyNameWithoutOverhead = GetAssemblyName(fullTypeName);
        var assemblyName = GetAssemblyNameWithoutOverhead(assemblyNameWithoutOverhead);
        var typeName = GetTypeName(fullTypeName);

        return FormatType(assemblyName, typeName);
    }

    /// <summary>
    /// Gets the name of the type without the assembly but including the namespace.
    /// </summary>
    /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
    /// <returns>The type name retrieved from the type, for example <c>Catel.TypeHelper</c>.</returns>
    /// <exception cref="ArgumentException">The <paramref name="fullTypeName"/> is <c>null</c> or whitespace.</exception>
    public static string GetTypeName(string fullTypeName) => ConvertTypeToVersionIndependentType(fullTypeName, true);

    /// <summary>
    ///   Formats a type in the official type description like [typename], [assemblyname].
    /// </summary>
    /// <param name = "assembly">Assembly name to format.</param>
    /// <param name = "type">Type name to format.</param>
    /// <returns>Type name like [typename], [assemblyname].</returns>
    /// <exception cref="ArgumentException">The <paramref name="assembly"/> is <c>null</c> or whitespace.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> is <c>null</c> or whitespace.</exception>
    public static string FormatType(string assembly, string type) => $"{type}, {assembly}";

    /// <summary>
    /// Converts a string representation of a type to a version independent type by removing the assembly version information.
    /// </summary>
    /// <param name="type">Type to convert.</param>
    /// <param name="stripAssemblies">if set to <c>true</c>, the assembly names will be stripped as well.</param>
    /// <returns>String representing the type without version information.</returns>
    /// <exception cref="ArgumentException">The <paramref name="type" /> is <c>null</c> or whitespace.</exception>
    public static string ConvertTypeToVersionIndependentType(string type, bool stripAssemblies = false)
    {
        const string innerTypesEnd = ",";

        var newType = type;
        var innerTypes = GetInnerTypes(newType);

        if (innerTypes.Length > 0)
        {
            // Remove inner types, but never strip assemblies because we need the real original type
            newType = newType.Replace($"[{FormatInnerTypes(innerTypes)}]", string.Empty, StringComparison.OrdinalIgnoreCase);
            for (var i = 0; i < innerTypes.Length; i++)
            {
                innerTypes[i] = ConvertTypeToVersionIndependentType(innerTypes[i], stripAssemblies);
            }
        }

        var splitterPos = newType.IndexOf(", ", StringComparison.Ordinal);
        var typeName = splitterPos != -1 ? newType[..splitterPos].Trim() : newType;
        var assemblyName = GetAssemblyName(newType);

        // Remove version info from assembly (if not signed by Microsoft)
        if (!string.IsNullOrWhiteSpace(assemblyName) && !stripAssemblies)
        {
            var isMicrosoftAssembly = MicrosoftPublicKeyTokens.Any(assemblyName.Contains);
            if (!isMicrosoftAssembly)
            {
                assemblyName = GetAssemblyNameWithoutOverhead(assemblyName);
            }

            newType = FormatType(assemblyName, typeName);
        }
        else
        {
            newType = typeName;
        }

        if (innerTypes.Length == 0) return newType;
        var innerTypesIndex = stripAssemblies ? newType.Length : newType.IndexOf(innerTypesEnd, StringComparison.Ordinal);
        if (innerTypesIndex >= 0)
        {
            newType = newType.Insert(innerTypesIndex, $"[{FormatInnerTypes(innerTypes, stripAssemblies)}]");
        }

        return newType;
    }

    /// <summary>
    /// Formats multiple inner types into one string.
    /// </summary>
    /// <param name="innerTypes">The inner types.</param>
    /// <param name="stripAssemblies">if set to <c>true</c>, the assembly names will be stripped as well.</param>
    /// <returns>string representing a combination of all inner types.</returns>
    public static string FormatInnerTypes(IEnumerable<string> innerTypes, bool stripAssemblies = false)
        => string.Join(",", innerTypes.Select(x =>
        {
            var type = stripAssemblies ? ConvertTypeToVersionIndependentType(x, true) : x;
            return $"[{type}]";
        }));

    /// <summary>
    /// Returns the inner type of a type, for example, a generic array type.
    /// </summary>
    /// <param name="type">Full type which might contain an inner type.</param>
    /// <returns>Array of inner types.</returns>
    /// <exception cref="ArgumentException">The <paramref name="type"/> is <c>null</c> or whitespace.</exception>
    [SuppressMessage("Roslynator", "RCS1075:Avoid empty catch clause that catches System.Exception", Justification = "Ignore all exceptions")]
    public static string[] GetInnerTypes(string type)
    {
        var innerTypes = new List<string>();

        try
        {
            var countIndex = type.IndexOf(InnerTypeCountStart, StringComparison.OrdinalIgnoreCase);
            if (countIndex == -1)
            {
                return [.. innerTypes];
            }

            // This is a generic, but does the type definition also contain the inner types?
            if (!type.Contains(AllTypesStart, StringComparison.OrdinalIgnoreCase))
            {
                return [.. innerTypes];
            }

            // Get the number of inner types
            var innerTypeCountEnd = -1;
            foreach (var t in InnerTypeCountEnd)
            {
                var index = type.IndexOf(t, StringComparison.OrdinalIgnoreCase);
                if (index != -1 && (innerTypeCountEnd == -1 || index < innerTypeCountEnd))
                {
                    // This value is more likely to be the one
                    innerTypeCountEnd = index;
                }
            }

            var innerTypeCount = int.Parse(type.AsSpan(countIndex + 1, innerTypeCountEnd - countIndex - 1), CultureInfo.InvariantCulture);

            // Remove all info until the first inner type
            if (!type.Contains(InternalTypeStart.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                // Just remove the info
                type = type[(innerTypeCountEnd + 1)..];
            }
            else
            {
                // Remove the index, but not the numbers
                var internalTypeEnd = type.IndexOf(InternalTypeEnd, StringComparison.OrdinalIgnoreCase);
                type = type[(internalTypeEnd + 1)..];
            }

            // Get all the inner types
            for (var i = 0; i < innerTypeCount; i++)
            {
                // Get the start & end of this inner type
                var innerTypeStart = type.IndexOf(SingleTypeStart, StringComparison.OrdinalIgnoreCase);
                var innerTypeEnd = innerTypeStart + 1;
                var openings = 1;

                // Loop until we find the end
                while (openings > 0)
                {
                    switch (type[innerTypeEnd])
                    {
                        case SingleTypeStart:
                            openings++;
                            break;
                        case SingleTypeEnd:
                            openings--;
                            break;
                    }

                    // Increase current pos if we still have openings left
                    if (openings > 0)
                    {
                        innerTypeEnd++;
                    }
                }

                innerTypes.Add(type.Substring(innerTypeStart + 1, innerTypeEnd - innerTypeStart - 1));
                type = type[(innerTypeEnd + 1)..];
            }
        }
        catch (Exception)
        {
            // Ignore exception
        }

        return [.. innerTypes];
    }

    /// <summary>
    /// Gets the <see cref="Type"/> from a string representation of the type. This method will try to find the type in all loaded assemblies, but also in the referenced assemblies of the loaded assemblies.
    /// </summary>
    /// <param name="valueType">The string representation of the type.</param>
    /// <returns>The <see cref="Type"/> if found; otherwise, <c>null</c>.</returns>
    public static Type? GetTypeFrom(string valueType)
    {
        // 1 - GetType
        var type = Type.GetType(valueType);
        if (type != null) return type;

        // 2 - Entry Assembly
        var entryAssembly = Assembly.GetEntryAssembly()!;
        type = entryAssembly.GetType(valueType);
        if (type != null) return type;

        // 3 - Other loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Except([entryAssembly]);

        // 4 - To speed things up, we check first in the already loaded assemblies.
        var list = assemblies.ToList();
        foreach (var assembly in list)
        {
            type = assembly.GetType(valueType);
            if (type != null) break;
        }

        if (type != null) return type;

        var loadedAssemblies = list.ToList();

        // 5 - Assemblies referenced but not loaded
        foreach (var loadedAssembly in list)
        {
            foreach (var referencedAssemblyName in loadedAssembly.GetReferencedAssemblies())
            {
                var found = loadedAssemblies.TrueForAll(x => x.GetName() != referencedAssemblyName);

                if (found) continue;
                try
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    type = referencedAssembly.GetType(valueType);
                    if (type != null)
                        break;
                    loadedAssemblies.Add(referencedAssembly);
                }
                catch
                {
                    // We will ignore this, because the Type might still be in one of the other Assemblies.
                }
            }
        }

        return type;
    }

    /// <summary>
    /// Gets the type name without the assembly namespace.
    /// </summary>
    /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
    /// <returns>The type name retrieved from the type, for example <c>TypeHelper</c>.</returns>
    public static string GetTypeNameWithoutNamespace(string fullTypeName)
    {
        fullTypeName = GetTypeName(fullTypeName);

        var splitterPos = fullTypeName.LastIndexOf('.');

        var typeName = splitterPos != -1 ? fullTypeName[(splitterPos + 1)..].Trim() : fullTypeName;
        return typeName;
    }

    /// <summary>
    /// Gets the type namespace.
    /// </summary>
    /// <param name="fullTypeName">Full name of the type, for example <c>Catel.TypeHelper, Catel.Core</c>.</param>
    /// <returns>The type namespace retrieved from the type, for example <c>Catel</c>.</returns>
    public static string GetTypeNamespace(string fullTypeName)
    {
        fullTypeName = GetTypeName(fullTypeName);

        var splitterPos = fullTypeName.LastIndexOf('.');

        var typeName = splitterPos != -1 ? fullTypeName[..splitterPos].Trim() : fullTypeName;
        return typeName;
    }
}
