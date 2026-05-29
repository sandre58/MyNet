// -----------------------------------------------------------------------
// <copyright file="IShellService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Combined shell facade for drawer and file menu operations.
/// </summary>
public interface IShellService : IShellDrawerService, IShellFileMenuService;
