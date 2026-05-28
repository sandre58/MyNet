// -----------------------------------------------------------------------
// <copyright file="FileExportByColumnsViewModelValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentValidation;
using MyNet.UI.Resources;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// FluentValidation rules for <see cref="FileExportByColumnsViewModelBase{T, TColumn}"/>.
/// </summary>
/// <typeparam name="T">The exported item type.</typeparam>
/// <typeparam name="TColumn">The column metadata type.</typeparam>
public class FileExportByColumnsViewModelValidator<T, TColumn> : AbstractValidator<FileExportByColumnsViewModelBase<T, TColumn>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileExportByColumnsViewModelValidator{T, TColumn}"/> class.
    /// </summary>
    public FileExportByColumnsViewModelValidator()
    {
        Include(new FileExportViewModelValidator<T>());

        RuleFor(x => x.Columns)
            .Must(columns => columns.Any(c => c.IsSelected))
            .WithMessage(_ => UiResources.ExportNoColumnsError);
    }
}
