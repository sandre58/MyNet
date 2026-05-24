// -----------------------------------------------------------------------
// <copyright file="PropertyDependencyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class PropertyDependencyTests
{
    [Fact]
    public void SetFilter_WhenItemPropertyChanges_ShouldReevaluateFilter()
    {
        var item = new FilterableItem { Value = 1 };
        using var collection = ExtendedCollection.From([item, new() { Value = 2 }]);

        collection.SetFilter(new ExpressionFilter<FilterableItem>(x => x.Value > 1));

        collection.Count.Should().Be(1);

        item.Value = 3;

        collection.Count.Should().Be(2);
    }

    [Fact]
    public void PropertyDependencyExtractor_ShouldCollectMemberNames()
    {
        var extractor = new PropertyDependencyExtractor<FilterableItem>();

        var deps = extractor.ExtractFilter(x => x.Value > 1 && x.Label.Length > 0);

        deps.Should().Contain("Value");
        deps.Should().Contain("Label");
    }

    private sealed class FilterableItem : INotifyPropertyChanged
    {
        public int Value
        {
            get;
            set => SetField(ref field, value);
        }

        public string Label
        {
            get;
        }

            = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
