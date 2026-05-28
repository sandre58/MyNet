// -----------------------------------------------------------------------
// <copyright file="MessageBoxViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Dialog;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// View model for a message box dialog. Button commands set a <see cref="MessageBoxResult"/> and request close.
/// </summary>
public sealed class MessageBoxViewModel : DialogViewModel<MessageBoxResult>, IMessageBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxViewModel"/> class.
    /// </summary>
    /// <param name="options">Message box configuration.</param>
    /// <param name="commandFactory">Optional command factory.</param>
    public MessageBoxViewModel(MessageBoxOptions options, ICommandFactory? commandFactory = null)
        : base(commandFactory)
    {
        ArgumentNullException.ThrowIfNull(options);

        Message = options.Message;
        Title = options.Title;
        Severity = options.Severity;
        Buttons = options.Buttons;
        DefaultResult = options.DefaultResult;

        var commands = commandFactory ?? RelayCommandFactory.Default;
        OkCommand = commands.Create(() => Complete(MessageBoxResult.Ok), () => Supports(MessageBoxResult.Ok));
        CancelCommand = commands.Create(() => Complete(MessageBoxResult.Cancel), () => Supports(MessageBoxResult.Cancel));
        YesCommand = commands.Create(() => Complete(MessageBoxResult.Yes), () => Supports(MessageBoxResult.Yes));
        NoCommand = commands.Create(() => Complete(MessageBoxResult.No), () => Supports(MessageBoxResult.No));
    }

    /// <inheritdoc />
    public string Message { get; }

    /// <inheritdoc />
    public MessageSeverity Severity { get; }

    /// <inheritdoc />
    public MessageBoxResultOption Buttons { get; }

    /// <inheritdoc />
    public MessageBoxResult DefaultResult { get; }

    /// <summary>Gets the command bound to the OK button when visible.</summary>
    public ICommand OkCommand { get; }

    /// <summary>Gets the command bound to the Cancel button when visible.</summary>
    public ICommand CancelCommand { get; }

    /// <summary>Gets the command bound to the Yes button when visible.</summary>
    public ICommand YesCommand { get; }

    /// <summary>Gets the command bound to the No button when visible.</summary>
    public ICommand NoCommand { get; }

    /// <summary>
    /// Applies a result without user interaction (headless presenter / tests).
    /// </summary>
    /// <param name="result">The result to apply.</param>
    public void ApplyResult(MessageBoxResult result) => Complete(result);

    private void Complete(MessageBoxResult result)
    {
        SetResult(result);
        RequestClose();
    }

    private bool Supports(MessageBoxResult result)
        => result switch
        {
            MessageBoxResult.Ok => Buttons is MessageBoxResultOption.Ok or MessageBoxResultOption.OkCancel,
            MessageBoxResult.Cancel => Buttons is MessageBoxResultOption.OkCancel or MessageBoxResultOption.YesNoCancel,
            MessageBoxResult.Yes => Buttons is MessageBoxResultOption.YesNo or MessageBoxResultOption.YesNoCancel,
            MessageBoxResult.No => Buttons is MessageBoxResultOption.YesNo or MessageBoxResultOption.YesNoCancel,
            _ => false
        };
}
