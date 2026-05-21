// -----------------------------------------------------------------------
// <copyright file="MessageBoxBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Builder for constructing and displaying message boxes with customizable options.
/// </summary>
/// <param name="service">The message box service used to display the message box.</param>
public sealed class MessageBoxBuilder(IMessageBoxService service) : IMessageBoxBuilder
{
    private readonly MessageBoxOptions _options = new();

    /// <inheritdoc />
    public IMessageBoxBuilder WithMessage(string message)
    {
        _options.Message = message;
        return this;
    }

    /// <inheritdoc />
    public IMessageBoxBuilder WithTitle(string title)
    {
        _options.Title = title;
        return this;
    }

    /// <inheritdoc />
    public IMessageBoxBuilder WithSeverity(MessageSeverity severity)
    {
        _options.Severity = severity;
        return this;
    }

    /// <inheritdoc />
    public IMessageBoxBuilder WithButtons(MessageBoxResultOption buttons)
    {
        _options.Buttons = buttons;
        return this;
    }

    /// <inheritdoc />
    public IMessageBoxBuilder WithDefaultResult(MessageBoxResult result)
    {
        _options.DefaultResult = result;
        return this;
    }

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowAsync(CancellationToken cancellationToken = default) => service.ShowAsync(_options, cancellationToken);
}
