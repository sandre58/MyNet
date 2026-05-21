// -----------------------------------------------------------------------
// <copyright file="IEditableRule.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace MyNet.UI.Legacy.ViewModels.Rules;

public interface IEditableRule : INotifyPropertyChanged
{
    bool CanRemove { get; }

    bool CanMove { get; }
}
