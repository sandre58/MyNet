// -----------------------------------------------------------------------
// <copyright file="ApplicationHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Reflection;
using MyNet.Primitives;
using MyNet.UI.Resources;

namespace MyNet.UI.Helpers;

public static class ApplicationHelper
{
    /// <summary>
    /// Gets the product name of the application from the assembly's product attribute. Returns an empty string if not found.
    /// </summary>
    /// <returns>The product name of the application.</returns>
    public static string GetProductName()
    {
        var assembly = Assembly.GetEntryAssembly();
        var productAttr = assembly?.GetCustomAttribute<AssemblyProductAttribute>();
        return productAttr?.Product ?? string.Empty;
    }

    /// <summary>
    /// Gets the version of the application from the assembly's version information. Returns an empty string if not found.
    /// </summary>
    /// <returns>The version of the application.</returns>
    public static string GetVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        return UiResources.VersionAbbrX.FormatWith(CultureInfo.CurrentCulture, assembly?.GetName().Version?.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Gets the copyright information of the application from the assembly's copyright attribute. Returns an empty string if not found.
    /// </summary>
    /// <returns>The copyright information of the application.</returns>
    public static string GetCopyright()
    {
        var assembly = Assembly.GetEntryAssembly();
        var attr = assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>();
        return attr?.Copyright ?? string.Empty;
    }

    /// <summary>
    /// Gets the company information of the application from the assembly's company attribute. Returns an empty string if not found.
    /// </summary>
    /// <returns>The company information of the application.</returns>
    public static string GetCompany()
    {
        var assembly = Assembly.GetEntryAssembly();
        var attr = assembly?.GetCustomAttribute<AssemblyCompanyAttribute>();
        return attr?.Company ?? string.Empty;
    }

    /// <summary>
    /// Gets the description of the application from the assembly's description attribute. Returns an empty string if not found.
    /// </summary>
    /// <returns>The description of the application.</returns>
    public static string GetDescription()
    {
        var assembly = Assembly.GetEntryAssembly();
        var attr = assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>();
        return attr?.Description ?? string.Empty;
    }
}
