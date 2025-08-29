﻿// -----------------------------------------------------------------------
// <copyright file="LoginDialogViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.ViewModels.Dialogs;

namespace MyNet.Avalonia.Demo.ViewModels.Dialogs;

internal class LoginDialogViewModel : DialogViewModel
{
    public string? Login { get; set; }

    public string? Password { get; set; }

    protected override string CreateTitle() => "Login";
}
