// -----------------------------------------------------------------------
// <copyright file="NumberExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class NumberExtensions
{
    public const int Ten = 10;
    public const int Hundred = 100;
    public const int Thousand = 1000;
    public const int Million = 1_000_000;
    public const int Billion = 1_000_000_000;

    extension(int value)
    {
        /// <summary>
        /// 5.Tens == 50.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Tens() => value * Ten;

        /// <summary>
        /// 4.Hundreds() == 400.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Hundreds() => value * Hundred;

        /// <summary>
        /// 3.Thousands() == 3000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Thousands() => value * Thousand;

        /// <summary>
        /// 2.Millions() == 2000000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Millions() => value * Million;

        /// <summary>
        /// 1.Billions() == 1000000000 (short scale).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Billions() => value * Billion;
    }

    extension(long value)
    {
        /// <summary>
        /// 5.Tens == 50.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Tens() => value * Ten;

        /// <summary>
        /// 4.Hundreds() == 400.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Hundreds() => value * Hundred;

        /// <summary>
        /// 3.Thousands() == 3000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Thousands() => value * Thousand;

        /// <summary>
        /// 2.Millions() == 2000000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Millions() => value * Million;

        /// <summary>
        /// 1.Billions() == 1000000000 (short scale).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Billions() => value * Billion;
    }

    extension(decimal value)
    {
        /// <summary>
        /// 5.Tens == 50.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal Tens() => value * Ten;

        /// <summary>
        /// 4.Hundreds() == 400.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal Hundreds() => value * Hundred;

        /// <summary>
        /// 3.Thousands() == 3000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal Thousands() => value * Thousand;

        /// <summary>
        /// 2.Millions() == 2000000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal Millions() => value * Million;

        /// <summary>
        /// 1.Billions() == 1000000000 (short scale).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal Billions() => value * Billion;
    }

    extension(double value)
    {
        /// <summary>
        /// 5.Tens == 50.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Tens() => value * Ten;

        /// <summary>
        /// 4.Hundreds() == 400.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Hundreds() => value * Hundred;

        /// <summary>
        /// 3.Thousands() == 3000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Thousands() => value * Thousand;

        /// <summary>
        /// 2.Millions() == 2000000.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Millions() => value * Million;

        /// <summary>
        /// 1.Billions() == 1000000000 (short scale).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Billions() => value * Billion;
    }
}
