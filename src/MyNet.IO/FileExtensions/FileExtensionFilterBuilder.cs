// -----------------------------------------------------------------------
// <copyright file="FileExtensionFilterBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.IO.FileExtensions;

/// <summary>
/// Represents a builder for creating a file extension filter, which is used to generate file filters for file dialogs. The builder allows adding multiple <see cref="FileExtensionGroup"/> items, which represent groups of file extensions. The resulting <see cref="FileExtensionFilter"/> can be used to create file filters that can be displayed in file dialogs, allowing users to filter files based on their extensions.
/// </summary>
public sealed class FileExtensionFilterBuilder
{
    private readonly List<FileExtensionGroup> _groups = [];

    /// <summary>
    /// Adds a <see cref="FileExtensionGroup"/> to the builder. This method allows you to specify a group of file extensions that will be included in the resulting <see cref="FileExtensionFilter"/>. Each group can represent a specific category of file extensions, and the builder can include multiple groups to create a comprehensive filter for file dialogs.
    /// </summary>
    /// <param name="group">The <see cref="FileExtensionGroup"/> to add to the builder.</param>
    /// <returns>The current instance of the <see cref="FileExtensionFilterBuilder"/>.</returns>
    public FileExtensionFilterBuilder Add(FileExtensionGroup group)
    {
        _groups.Add(group);
        return this;
    }

    /// <summary>
    /// Adds a range of <see cref="FileExtensionGroup"/> items to the builder. This method allows you to specify multiple groups of file extensions that will be included in the resulting <see cref="FileExtensionFilter"/>. Each group can represent a specific category of file extensions, and the builder can include multiple groups to create a comprehensive filter for file dialogs.
    /// </summary>
    /// <param name="groups">The collection of <see cref="FileExtensionGroup"/> items to add to the builder.</param>
    /// <returns>The current instance of the <see cref="FileExtensionFilterBuilder"/>.</returns>
    public FileExtensionFilterBuilder AddRange(IEnumerable<FileExtensionGroup> groups)
    {
        _groups.AddRange(groups);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="FileExtensionFilter"/> using the added <see cref="FileExtensionGroup"/> items. This method creates a new instance of <see cref="FileExtensionFilter"/> containing all the groups added to the builder.
    /// </summary>
    /// <returns>A new instance of <see cref="FileExtensionFilter"/> containing the added groups.</returns>
    public FileExtensionFilter Build() => new(_groups);
}
