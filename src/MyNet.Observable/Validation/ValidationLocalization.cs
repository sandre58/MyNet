// -----------------------------------------------------------------------
// <copyright file="ValidationLocalization.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using FluentValidation;
using MyNet.Globalization.Static;
using MyNet.Observable.Localization;

namespace MyNet.Observable.Validation;

/// <summary>
/// Configures FluentValidation to use <see cref="ValidationResources"/> and translated property names.
/// </summary>
public static class ValidationLocalization
{
    private static int _configured;

    /// <summary>
    /// Replaces the global FluentValidation language manager and display name resolver.
    /// Safe to call once at application startup.
    /// </summary>
    public static void Configure()
    {
        if (Interlocked.Exchange(ref _configured, 1) == 1)
            return;

        ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();
        ValidatorOptions.Global.DisplayNameResolver = ResolveDisplayName;
    }

    /// <summary>
    /// Assigns <see cref="ValidationLanguageManager"/> without changing the display name resolver.
    /// </summary>
    public static void UseValidationResourcesLanguageManager()
        => ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();

    /// <summary>
    /// Resolves property display names from the member name translation key.
    /// </summary>
    public static string? ResolveDisplayName(Type type, MemberInfo? member, LambdaExpression expression)
    {
        _ = type;
        _ = expression;

        if (member is null)
            return null;

        var translated = member.Name.Translate();
        return string.IsNullOrWhiteSpace(translated) ? member.Name : translated;
    }
}
