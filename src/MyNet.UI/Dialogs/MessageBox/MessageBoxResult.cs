// -----------------------------------------------------------------------
// <copyright file="MessageBoxResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.MessageBox;

// Summary:
//     Specifies which message box button that a user clicks. System.Windows.MessageBoxResult
//     is returned by the Overload:System.Windows.MessageBox.Show method.
public enum MessageBoxResult
{
    // Summary:
    //     The message box returns no result.
    None,

    // Summary:
    //     The result value of the message box is OK.
    Ok,

    // Summary:
    //     The result value of the message box is Cancel.
    Cancel,

    // Summary:
    //     The result value of the message box is Yes.
    Yes,

    // Summary:
    //     The result value of the message box is No.
    No
}
