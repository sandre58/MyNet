// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using MyNet.IO.FileHistory;
using MyNet.UI.Services.FileHistory;

#pragma warning disable IDE0130
namespace MyNet.UI.Services;
#pragma warning restore IDE0130

/// <summary>
/// Registers recent-files UI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IRecentFilesOperations"/> backed by <see cref="IRecentFilesService"/>.
    /// </summary>
    public static IServiceCollection AddRecentFilesOperations(this IServiceCollection services) =>
        services.AddSingleton<IRecentFilesOperations, RecentFilesOperations>();
}
