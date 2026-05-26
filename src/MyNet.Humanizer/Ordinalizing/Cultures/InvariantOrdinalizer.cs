// -----------------------------------------------------------------------
// <copyright file="InvariantOrdinalizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Humanizer.Ordinalizing.Cultures;

/// <summary>
/// Represents an ordinalizer that uses the invariant culture and has no specific ordinalization rules. This class can be used as a default or placeholder ordinalizer when no specific culture or ordinalization rules are needed, effectively treating all numbers as unordinalized and using the invariant culture for any culture-specific operations related to ordinalization.
/// </summary>
public sealed class InvariantOrdinalizer() : OrdinalizerBase(CultureInfo.InvariantCulture);
