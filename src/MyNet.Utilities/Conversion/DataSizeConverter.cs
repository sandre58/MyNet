// -----------------------------------------------------------------------
// <copyright file="DataSizeConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Conversion;

/// <summary>
/// Provides a converter for file size units, allowing conversion between different file size units such as bytes, kilobytes, megabytes, etc. The conversion is based on powers of 1024, which is the standard for file size measurements in computing. This converter can be used to easily convert file sizes from one unit to another without having to manually calculate the conversion factors.
/// </summary>
public sealed class DataSizeConverter : IUnitConverter<DataSizeUnit>
{
    /// <summary>
    /// Converts a file size value from one unit to another. The conversion is performed by calculating the difference in unit levels and applying the appropriate power of 1024 to convert the value accordingly. If the target unit is smaller than the source unit, the value is multiplied by the corresponding power of 1024; if the target unit is larger, the value is divided by the corresponding power of 1024. If both units are the same, the original value is returned unchanged.
    /// </summary>
    /// <param name="value">The file size value to convert.</param>
    /// <param name="from">The unit of the input value.</param>
    /// <param name="to">The unit to convert the value to.</param>
    /// <returns>The converted file size value.</returns>
    public double Convert(double value, DataSizeUnit from, DataSizeUnit to)
    {
        var diff = to - from;

        return diff switch
        {
            0 => value,
            > 0 => value / Pow1024(diff),
            _ => value * Pow1024(-diff)
        };
    }

    /// <summary>
    /// Calculates the power of 1024 for a given exponent. This method multiplies 1024 by itself the specified number of times to compute the result. It is used to determine the conversion factor between different file size units based on their relative positions in the enumeration. For example, if the exponent is 1, it returns 1024; if the exponent is 2, it returns 1024 squared (1,048,576), and so on.
    /// </summary>
    /// <param name="exp">The exponent to raise 1024 to.</param>
    /// <returns>The result of 1024 raised to the specified exponent.</returns>
    private static double Pow1024(int exp)
    {
        double result = 1;
        const double baseValue = 1024;

        for (var i = 0; i < exp; i++)
            result *= baseValue;

        return result;
    }
}
