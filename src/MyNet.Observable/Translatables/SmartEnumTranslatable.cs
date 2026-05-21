// -----------------------------------------------------------------------
// <copyright file="SmartEnumTranslatable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer;
using MyNet.Observable.Attributes;
using MyNet.Utilities;

namespace MyNet.Observable.Translatables;

public class SmartEnumTranslatable(ISmartEnum enumValue) : SmartEnumTranslatable<ISmartEnum>(enumValue);

public class SmartEnumTranslatable<TEnum>(TEnum enumValue) : Translatable<TEnum>(() => enumValue)
    where TEnum : ISmartEnum
{
    [UpdateOnCultureChanged]
    public string Display => Value?.Humanize() ?? string.Empty;

    public override string ToString() => Display;

    public override bool Equals(object? obj) => obj is SmartEnumTranslatable<TEnum> result && (result.Value?.Equals(Value) ?? false);

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
}
