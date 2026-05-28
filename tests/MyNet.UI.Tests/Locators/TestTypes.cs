// -----------------------------------------------------------------------
// <copyright file="TestTypes.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable CheckNamespace
// These types are in specific namespaces so that assembly.GetType(fullName) can resolve them
// during convention-based resolution tests.
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130
namespace MyNet.UI.Tests.ViewModels
#pragma warning restore IDE0130
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "These types are in specific namespaces so that assembly.GetType(fullName) can resolve them during convention-based resolution tests.")]
    internal class PersonViewModel;

    internal class ItemViewModel;

    internal class DashboardViewModel;
}

#pragma warning disable SA1403, IDE0130
namespace MyNet.UI.Tests.Views
#pragma warning restore IDE0130, SA1403
{
    internal class PersonView;

    internal class ItemView;

    internal class DashboardControl;

    internal class DashboardWindow;

    internal class DashboardPage;

    internal class DashboardFragment;
}
