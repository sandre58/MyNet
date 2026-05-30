// -----------------------------------------------------------------------
// <copyright file="ViewModelExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.ViewModels;
using MyNet.UI.ViewModels.Crud;
using Xunit;

namespace MyNet.UI.Tests.ViewModels;

public sealed class ViewModelExtensionsTests
{
    [Fact]
    public void ObserveItemChanged_InvokesHandlerAndUnsubscribesOnDispose()
    {
        var vm = new TestItemViewModel();
        string? observed = null;

        using (vm.ObserveItemChanged(value => observed = value))
        {
            vm.RaiseItemChanged("updated");
        }

        observed.Should().Be("updated");

        vm.RaiseItemChanged("ignored");
        observed.Should().Be("updated");
    }

    private sealed class TestItemViewModel : IItemViewModel<string>
    {
        public string? Item { get; private set; }

#pragma warning disable CS0067
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        public event System.EventHandler<ItemChangedEventArgs<string>>? ItemChanged;

        public void RaiseItemChanged(string value)
        {
            Item = value;
            ItemChanged?.Invoke(this, new(null, value));
        }
    }
}
