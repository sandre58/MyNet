// -----------------------------------------------------------------------
// <copyright file="ImportDialogViewModelValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentValidation;
using MyNet.UI.Resources;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// FluentValidation rules for <see cref="ImportDialogViewModel{T}"/>.
/// </summary>
/// <typeparam name="T">The import item type.</typeparam>
public class ImportDialogViewModelValidator<T> : AbstractValidator<ImportDialogViewModel<T>>
    where T : ImportItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImportDialogViewModelValidator{T}"/> class.
    /// </summary>
    public ImportDialogViewModelValidator()
    {
        RuleFor(x => x.ImportItemCount)
            .GreaterThan(0)
            .WithMessage(_ => UiResources.ImportNoItemsError);

        RuleFor(x => x)
            .Custom((vm, context) =>
            {
                foreach (var error in vm.List.ImportItems
                             .SelectMany(item => item.ValidateForImport())
                             .Where(message => !string.IsNullOrWhiteSpace(message)))
                {
                    context.AddFailure(error);
                }
            });
    }
}
