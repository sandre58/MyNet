// -----------------------------------------------------------------------
// <copyright file="ValidationGraphVisitorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using MyNet.Observable.Behaviors;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class ValidationGraphVisitorTests
{
    [Fact]
    public void Validate_NullRoot_ReturnsTrue() => Assert.True(ValidationGraphVisitor.Validate(null));

    [Fact]
    public void Validate_InvalidRoot_ReturnsFalse() => Assert.False(ValidationGraphVisitor.Validate(new StubValidatable(isValid: false)));

    [Fact]
    public void Validate_InvalidItemInCollection_ReturnsFalse()
    {
        var graph = new NodeWithItems();

        Assert.False(ValidationGraphVisitor.Validate(graph));
    }

    [Fact]
    public void Validate_CyclicGraph_DoesNotStackOverflow()
    {
        var left = new CyclicNode();
        var right = new CyclicNode();
        left.Other = right;
        right.Other = left;
        left.Node = new(isValid: true);

        Assert.True(ValidationGraphVisitor.Validate(left));
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used for testing purposes only.")]
    private sealed class NodeWithItems
    {
        public List<StubValidatable> Items { get; } = [new(isValid: false)];
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used for testing purposes only.")]
    private sealed class CyclicNode
    {
        public CyclicNode? Other { get; set; }

        public StubValidatable? Node { get; set; }
    }

    private sealed class StubValidatable(bool isValid) : IValidationAware
    {
        public IReadOnlyCollection<string> Errors { get; } = [];

        public bool HasErrors => !isValid;

        event EventHandler<DataErrorsChangedEventArgs>? INotifyDataErrorInfo.ErrorsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable GetErrors(string? propertyName) => Errors;

        public bool Validate() => isValid;

        public void ValidateProperty(string propertyName) { }

        public void ResetValidation() { }
    }
}
