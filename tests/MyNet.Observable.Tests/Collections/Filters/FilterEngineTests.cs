// -----------------------------------------------------------------------
// <copyright file="FilterEngineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable.Collections.Filters;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Filters;

public sealed class FilterEngineTests
{
    [Fact]
    public void Set_UpdatesPredicate()
    {
        using var engine = new FilterEngine<int>();
        Func<int, bool>? predicate = null;
        using var subscription = engine.Predicate.Subscribe(p => predicate = p);

        var filter = FilterBuilder<int>.Create().Where(x => x >= 5).Build();
        engine.Set(filter!);

        Assert.NotNull(predicate);
        Assert.True(predicate!(10));
        Assert.False(predicate!(2));
    }

    [Fact]
    public void Clear_ResetsToMatchAll()
    {
        using var engine = new FilterEngine<int>();
        Func<int, bool>? predicate = null;
        using var subscription = engine.Predicate.Subscribe(p => predicate = p);

        engine.Set(FilterBuilder<int>.Create().Where(_ => false).Build()!);
        engine.Clear();

        Assert.NotNull(predicate);
        Assert.True(predicate!(42));
        Assert.Null(engine.Current);
    }

    [Fact]
    public void Invalidate_ReEmitsCurrentPredicate()
    {
        using var engine = new FilterEngine<int>();
        var emitCount = 0;

        var filter = FilterBuilder<int>.Create().Where(x => x > 0).Build();
        engine.Set(filter!);

        using var subscription = engine.Predicate.Subscribe(_ => emitCount++);
        engine.Invalidate();

        Assert.Equal(2, emitCount);
    }
}
