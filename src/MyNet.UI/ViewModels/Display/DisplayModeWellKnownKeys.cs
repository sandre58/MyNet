// -----------------------------------------------------------------------
// <copyright file="DisplayModeWellKnownKeys.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Stable string identifiers for built-in display modes.
/// Use these values when persisting user preferences, migrating legacy settings, or matching <see cref="IDisplayModeViewModel.Key"/>.
/// </summary>
/// <remarks>
/// Key spellings (including <see cref="Detailed"/>) are frozen for backward compatibility with persisted data.
/// </remarks>
public static class DisplayModeWellKnownKeys
{
    /// <summary>Grid / tile layout.</summary>
    public const string Grid = "DisplayModeGrid";

    /// <summary>Detailed item layout (legacy spelling: Detailled).</summary>
    public const string Detailed = "DisplayModeDetailled";

    /// <summary>Chart visualization.</summary>
    public const string Chart = "DisplayModeChart";

    /// <summary>List with column options.</summary>
    public const string List = "DisplayModeList";

    /// <summary>Hour-based calendar.</summary>
    public const string Hour = "DisplayModeHour";

    /// <summary>Day-based calendar.</summary>
    public const string Day = "DisplayModeDay";

    /// <summary>Week-based calendar.</summary>
    public const string Week = "DisplayModeWeek";

    /// <summary>Month-based calendar.</summary>
    public const string Month = "DisplayModeMonth";

    /// <summary>Year-based calendar.</summary>
    public const string Year = "DisplayModeYear";
}
