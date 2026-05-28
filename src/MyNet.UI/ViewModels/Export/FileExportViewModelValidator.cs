// -----------------------------------------------------------------------
// <copyright file="FileExportViewModelValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MyNet.Observable;
using MyNet.UI.Resources;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// FluentValidation rules for <see cref="FileExportViewModelBase{T}"/>.
/// </summary>
/// <typeparam name="T">The exported item type.</typeparam>
public class FileExportViewModelValidator<T> : AbstractValidator<FileExportViewModelBase<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileExportViewModelValidator{T}"/> class.
    /// </summary>
    public FileExportViewModelValidator()
    {
        Include(new ExportViewModelValidator<T>());

        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithRequiredLocalizedMessage(nameof(FileExportViewModelBase<>.Destination));

        RuleFor(x => x.Destination)
            .Must((vm, destination) => vm.FileType.IsValidPath(destination!))
            .When(x => !string.IsNullOrWhiteSpace(x.Destination))
            .WithMessage(_ => MessageResources.FileHasInvalidExtensionError);
    }
}
