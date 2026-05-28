// -----------------------------------------------------------------------
// <copyright file="ExportViewModelValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MyNet.UI.Resources;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// FluentValidation rules for <see cref="ExportViewModelBase{T}"/>.
/// </summary>
/// <typeparam name="T">The exported item type.</typeparam>
public class ExportViewModelValidator<T> : AbstractValidator<ExportViewModelBase<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExportViewModelValidator{T}"/> class.
    /// </summary>
    public ExportViewModelValidator() => RuleFor(x => x.ExportItemCount)
        .GreaterThan(0)
        .WithMessage(_ => UiResources.ExportNoItemsError);
}
