// -----------------------------------------------------------------------
// <copyright file="IConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Converters;

/// <summary>
/// Defines a bidirectional converter between two types.
/// Implementations provide logic to convert from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>
/// and to convert back from <typeparamref name="TTo"/> to <typeparamref name="TFrom"/>.
/// </summary>
/// <typeparam name="TFrom">Source type for conversion.</typeparam>
/// <typeparam name="TTo">Target type for conversion.</typeparam>
public interface IConverter<TFrom, TTo>
{
    /// <summary>
    /// Converts the specified source item to the target type.
    /// </summary>
    /// <param name="item">The source item to convert.</param>
    /// <returns>The converted item of type <typeparamref name="TTo"/>.</returns>
    TTo Convert(TFrom item);

    /// <summary>
    /// Converts the specified target item back to the source type.
    /// </summary>
    /// <param name="item">The target item to convert back.</param>
    /// <returns>The converted item of type <typeparamref name="TFrom"/>.</returns>
    TFrom ConvertBack(TTo item);
}
