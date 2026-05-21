// -----------------------------------------------------------------------
// <copyright file="SelectionEngineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DynamicData;
using FluentAssertions;
using MyNet.Observable.Collections.Selection;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Selection;

public sealed class SelectionEngineTests
{
    private static (SourceList<string> Source, SelectionEngine<string> Engine) Create(SelectionMode mode = SelectionMode.Multiple)
    {
        var source = new SourceList<string>();
        var engine = new SelectionEngine<string>(source.Connect(), mode);
        return (source, engine);
    }

    // ── Select ────────────────────────────────────────────────────────────────
    [Fact]
    public void Select_ShouldSelectItem()
    {
        var (src, engine) = Create();
        src.Edit(l => l.Add("a"));

        engine.Select("a");

        engine.SelectedItems.Should().ContainSingle().Which.Should().Be("a");
    }

    [Fact]
    public void Select_ItemNotInSource_ShouldBeIgnored()
    {
        var (_, engine) = Create();

        engine.Select("ghost");

        engine.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void Select_Multiple_ShouldAccumulateInMultipleMode()
    {
        var (src, engine) = Create();
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
            l.Add("c");
        });

        engine.Select("a");
        engine.Select("b");

        engine.SelectedItems.Should().BeEquivalentTo(["a", "b"]);
    }

    [Fact]
    public void Select_InSingleMode_ShouldReplaceSelection()
    {
        var (src, engine) = Create(SelectionMode.Single);
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
        });

        engine.Select("a");
        engine.Select("b");

        engine.SelectedItems.Should().ContainSingle().Which.Should().Be("b");
    }

    // ── Unselect ──────────────────────────────────────────────────────────────
    [Fact]
    public void Unselect_ShouldRemoveItem()
    {
        var (src, engine) = Create();
        src.Edit(l => l.Add("a"));
        engine.Select("a");

        engine.Unselect("a");

        engine.SelectedItems.Should().BeEmpty();
    }

    // ── Toggle ────────────────────────────────────────────────────────────────
    [Fact]
    public void Toggle_ShouldSelectIfNotSelected()
    {
        var (src, engine) = Create();
        src.Edit(l => l.Add("a"));

        engine.Toggle("a");

        engine.SelectedItems.Should().ContainSingle();
    }

    [Fact]
    public void Toggle_ShouldDeselectIfAlreadySelected()
    {
        var (src, engine) = Create();
        src.Edit(l => l.Add("a"));
        engine.Select("a");

        engine.Toggle("a");

        engine.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void Toggle_InSingleMode_ShouldClearOthersOnSelect()
    {
        var (src, engine) = Create(SelectionMode.Single);
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
        });
        engine.Select("a");

        engine.Toggle("b");

        engine.SelectedItems.Should().ContainSingle().Which.Should().Be("b");
    }

    [Fact]
    public void Toggle_InSingleMode_ShouldDeselectIfAlreadySelected()
    {
        var (src, engine) = Create(SelectionMode.Single);
        src.Edit(l => l.Add("a"));
        engine.Select("a");

        engine.Toggle("a");

        engine.SelectedItems.Should().BeEmpty();
    }

    // ── Set ───────────────────────────────────────────────────────────────────
    [Fact]
    public void Set_Multiple_ShouldReplaceSelection()
    {
        var (src, engine) = Create();
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
            l.Add("c");
        });
        engine.Select("a");

        engine.Set(["b", "c"]);

        engine.SelectedItems.Should().BeEquivalentTo(["b", "c"]);
    }

    [Fact]
    public void Set_Single_ShouldSelectOnlyFirstValidItem()
    {
        var (src, engine) = Create(SelectionMode.Single);
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
            l.Add("c");
        });

        engine.Set(["b", "c"]);

        engine.SelectedItems.Should().ContainSingle().Which.Should().Be("b");
    }

    [Fact]
    public void Set_WithItemNotInSource_ShouldIgnoreIt()
    {
        var (src, engine) = Create();
        src.Edit(l => l.Add("a"));

        engine.Set(["a", "ghost"]);

        engine.SelectedItems.Should().ContainSingle().Which.Should().Be("a");
    }

    // ── ClearRules ─────────────────────────────────────────────────────────────────
    [Fact]
    public void Clear_ShouldEmptySelection()
    {
        var (src, engine) = Create();
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
        });
        engine.Set(["a", "b"]);

        engine.Clear();

        engine.SelectedItems.Should().BeEmpty();
    }

    // ── Source changes purge selection ────────────────────────────────────────
    [Fact]
    public void Remove_SelectedItem_ShouldPurgeFromSelection()
    {
        var (src, engine) = Create();
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
        });
        engine.Set(["a", "b"]);

        src.Edit(l => l.Remove("a"));

        engine.SelectedItems.Should().ContainSingle().Which.Should().Be("b");
    }

    [Fact]
    public void ClearSource_ShouldPurgeAllSelection()
    {
        var (src, engine) = Create();
        src.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
            l.Add("c");
        });
        engine.Set(["a", "b", "c"]);

        src.Edit(l => l.Clear());

        engine.SelectedItems.Should().BeEmpty();
    }

    // ── Observable ────────────────────────────────────────────────────────────
    [Fact]
    public void Connect_ShouldEmitOnSelectionChange()
    {
        var (src, engine) = Create();
        src.Edit(l => l.Add("a"));

        var emitted = new List<IReadOnlyCollection<string>>();
        using var sub = engine.Connect().Subscribe(onNext);

        engine.Select("a");
        engine.Unselect("a");

        emitted.Should().HaveCountGreaterThanOrEqualTo(2);
        return;
        void onNext(IReadOnlyCollection<string> items) => emitted.Add(items);
    }
}
