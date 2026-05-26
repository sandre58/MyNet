// -----------------------------------------------------------------------
// <copyright file="TimeOnlyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TimeOnlyExtensions
{
    extension(TimeOnly time)
    {
        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public TimeOnly BeginningOfHour() => new(time.Hour, 0, 0, 0);

        /// <summary>
        /// Returns the very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public TimeOnly EndOfHour() => new(time.Hour, 59, 59, 999);

        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public TimeOnly BeginningOfMinute() => new(time.Hour, time.Minute, 0, 0);

        /// <summary>
        /// Returns the very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public TimeOnly EndOfMinute() => new(time.Hour, time.Minute, 59, 999);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Hour part.
        /// </summary>
        public TimeOnly SetHour(int hour) => new(hour, time.Minute, time.Second, time.Millisecond);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Minute part.
        /// </summary>
        public TimeOnly SetMinute(int minute) => new(time.Hour, minute, time.Second, time.Millisecond);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Second part.
        /// </summary>
        public TimeOnly SetSecond(int second) => new(time.Hour, time.Minute, second, time.Millisecond);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Millisecond part.
        /// </summary>
        public TimeOnly SetMillisecond(int millisecond) => new(time.Hour, time.Minute, time.Second, millisecond);

        /// <summary>
        /// Determines if the given <see cref="DateTime"/> has the same Hour, Minute, Second and Millisecond parts as the current <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to compare with.</param>
        /// <returns><c>true</c> if the specified <see cref="DateTime"/> has the same Hour, Minute, Second and Millisecond parts; otherwise, <c>false</c>.</returns>
        public bool SameMilliSecond(TimeOnly date) => time.SameSecond(date) && time.Millisecond == date.Millisecond;

        /// <summary>
        /// Determines if the given <see cref="DateTime"/> has the same Hour, Minute and Second parts as the current <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to compare with.</param>
        /// <returns><c>true</c> if the specified <see cref="DateTime"/> has the same Hour, Minute, Second and Millisecond parts; otherwise, <c>false</c>.</returns>
        public bool SameSecond(TimeOnly date) => time.SameMinute(date) && time.Second == date.Second;

        /// <summary>
        /// Determines if the given <see cref="DateTime"/> has the same Hour and Minute parts as the current <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to compare with.</param>
        /// <returns><c>true</c> if the specified <see cref="DateTime"/> has the same Hour and Minute parts; otherwise, <c>false</c>.</returns>
        public bool SameMinute(TimeOnly date) => time.SameHour(date) && time.Minute == date.Minute;

        /// <summary>
        /// Determines if the given <see cref="DateTime"/> has the same Hour part as the current <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to compare with.</param>
        /// <returns><c>true</c> if the specified <see cref="DateTime"/> has the same Hour part; otherwise, <c>false</c>.</returns>
        public bool SameHour(TimeOnly date) => time.Hour == date.Hour;

        /// <summary>
        ///  Generates a sequence of TimeOnly values starting from a minimum value and ending at a maximum value, with a specified step and time unit. The method first checks if the step is zero and throws an exception if it is. It then defines an increment function based on the specified time unit, which will be used to generate the next value in the sequence. Finally, it uses a loop to yield each value in the range until it reaches the maximum value, taking into account whether the step is positive or negative. Note that only Millisecond, Second, Minute, and Hour time units are valid for TimeOnly values; using other time units will throw an InvalidOperationException.
        /// </summary>
        /// <param name="max">The maximum value of the range.</param>
        /// <param name="step">The step value for each iteration.</param>
        /// <param name="unit">The time unit for the step.</param>
        /// <returns>A sequence of TimeOnly values within the specified range.</returns>
        public IEnumerable<TimeOnly> Range(TimeOnly max, int step = 1, TimeUnit unit = TimeUnit.Hour)
        {
            ArgumentOutOfRangeException.ThrowIfZero(step);

            Func<TimeOnly, TimeOnly> increment = unit switch
            {
                TimeUnit.Millisecond => x => x.Add(step.Milliseconds()),
                TimeUnit.Second => x => x.Add(step.Seconds()),
                TimeUnit.Minute => x => x.AddMinutes(step),
                TimeUnit.Hour => x => x.AddHours(step),
                TimeUnit.Day => throw new InvalidOperationException(),
                TimeUnit.Week => throw new InvalidOperationException(),
                TimeUnit.Month => throw new InvalidOperationException(),
                TimeUnit.Year => throw new InvalidOperationException(),
                _ => x => x.AddHours(step)
            };

            if (step > 0)
            {
                for (var i = time; i <= max; i = increment(i))
                    yield return i;
            }
            else
            {
                for (var i = time; i >= max; i = increment(i))
                    yield return i;
            }
        }
    }
}
