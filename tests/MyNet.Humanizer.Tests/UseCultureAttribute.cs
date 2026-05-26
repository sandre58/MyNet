// -----------------------------------------------------------------------
// <copyright file="UseCultureAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using MyNet.Globalization.Static;
using Xunit.Sdk;

namespace MyNet.Humanizer.Tests;

/// <summary>
/// Apply this attribute to your test method to replace the
/// <see cref="Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
/// <see cref="CultureInfo.CurrentUICulture" /> with another culture.
/// </summary>
/// <remarks>
/// Replaces the culture and UI culture of the current thread with
/// <paramref name="cultureName" />.
/// </remarks>
/// <param name="cultureName">The name of the culture.</param>
/// <remarks>
/// <para>
/// This constructor overload uses <paramref name="cultureName" /> for both
/// <see cref="CultureName" /> and <see cref="CultureInfo.CurrentUICulture" />.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class UseCultureAttribute(string cultureName) : BeforeAfterTestAttribute
{
    private CultureInfo? _originalCulture;

    public string CultureName { get; } = cultureName;

    /// <summary>
    /// Stores the current <see cref="CultureInfo.CurrentCulture" />
    /// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
    /// and replaces them with the new cultures defined in the constructor.
    /// </summary>
    /// <param name="methodUnderTest">The method under test.</param>
    public override void Before(MethodInfo methodUnderTest)
    {
        _originalCulture = CultureInfo.CurrentCulture;

        GlobalizationServices.Current.SetCulture(new CultureInfo(CultureName));
    }

    /// <summary>
    /// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
    /// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="CultureInfo.CurrentCulture" />.
    /// </summary>
    /// <param name="methodUnderTest">The method under test.</param>
    public override void After(MethodInfo methodUnderTest)
    {
        if (_originalCulture == null) return;

        GlobalizationServices.Current.SetCulture(_originalCulture);
    }
}
