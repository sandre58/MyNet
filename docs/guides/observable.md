# Observable models

**Package:** [MyNet.Observable](../../src/MyNet.Observable/README.md)

MVVM-oriented **observable**, **editable**, and **validatable** models with composable behaviors and a Roslyn source generator.

The package references **MyNet.Observable.Metadata.Generator** as an analyzer (not a separate NuGet package): metadata bootstrap, `[ObservableProperty]`, and setter usage diagnostics.

## Installation

```bash
dotnet add package MyNet.Observable
```

---

## Observable properties

### Recommended: `[ObservableProperty]`

Declare a **partial** type and mark backing fields:

```csharp
using MyNet.Observable;
using MyNet.Observable.Metadata;

public partial class PersonViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
}
```

The generator emits a public property whose setter calls `SetProperty(ref _name, value)`.

### Alternative: explicit setters

```csharp
public string Name
{
    get => _name;
    set => SetProperty(ref _name, value);
}
```

`SetProperty`:

- runs **PropertyChanging → assign → PropertyChanged**
- honors **`Cancel`** from behaviors and `OnPropertyChangingCore`
- works with **notification suspension** (coalescing)

### Manual changing pipeline (advanced)

When you need custom logic before assign:

```csharp
set
{
    var before = _name;
    if (!OnPropertyChanging(nameof(Name), before, value))
        return;
    _name = value;
    OnPropertyChanged(nameof(Name), before, value);
}
```

Avoid assigning a backing field and calling only `OnPropertyChanged` — analyzer **MNETOBS004** warns when assignment bypasses `SetProperty` or the changing pipeline.

---

## Behaviors (composition)

Register capabilities on **`Behaviors`** (not on `ObservableObject` directly):

```csharp
using MyNet.Observable;
using MyNet.Globalization.Culture;

public sealed class PersonViewModel : ObservableObject
{
    public PersonViewModel(ICultureService cultureService)
    {
        this.ReactOnCultureChanged(cultureService);
        this.UseTracking();
        this.UseValidation(new PersonValidator());
    }
}
```

| Extension | Purpose |
|-----------|---------|
| `UseTracking()` | Dirty / modification tracking |
| `UseValidation(validator)` | FluentValidation + `INotifyDataErrorInfo` |
| `ReactOnCultureChanged(service)` | Refresh culture-sensitive properties |
| `ReactOnTimeZoneChanged(service)` | Refresh time-zone-sensitive properties |
| `ForwardProperty(x => x.Child)` | Relay child `INotifyPropertyChanged` |

API: `vm.Behaviors.Register`, `Get`, `TryGet`, `GetAll`, `Unregister`.

---

## Property metadata (attributes)

On **properties** of `ObservableObject` subclasses:

```csharp
using MyNet.Observable.Behaviors.Metadata.Attributes;

[UpdateOnCultureChanged]
public string DisplayName { get; set; } = string.Empty;

[IgnoreModificationTracking]
public int InternalId { get; set; }

[ForwardProperty]
public AddressViewModel Address { get; set; } = null!;
```

At compile time, the generator emits lazy bootstrap code. The first `MetadataRegistry.Get(typeof(YourType))` applies configuration. `ObservableObject` reads forwarding metadata in its constructor and registers `PropertyChangedForwardingBehavior`.

See [Metadata generation](#metadata-generation) for the full pipeline.

---

## Strict metadata mode (optional)

```csharp
using MyNet.Observable.Metadata;

[assembly: EnforceGeneratedMetadata]
```

Types deriving from `ObservableObject` without metadata produce **MNETMETA001**. Opt out with `[ExemptFromGeneratedMetadata]` on intentional bare types.

---

## Notification suspension

```csharp
using (SuspendNotifications()) // CoalesceOnResume (default)
{
    Name = "a";
    Name = "b";
} // one PropertyChanged for Name (first old → last new)

using (SuspendNotifications(NotificationSuspensionMode.Drop))
{
    Name = "x"; // suppressed, not replayed
}
```

---

## Extended collections (filter, sort, group, selection)

`ExtendedCollection<T>` is the reactive list pipeline built on DynamicData. It exposes:

- `Items` — filtered and sorted view (bind list UI here)
- `Source` / `SourceCount` — sorted source before filtering
- `Count` — filtered item count (`PropertyChanged` when the view changes, via `SetProperty`)
- `SetFilter` / `SetSorting` / `SetGrouping` — runtime pipeline configuration

### Factory

```csharp
using MyNet.Observable.Collections;

using var list = ExtendedCollection.From(people);
list.SetFilter(new ExpressionFilter<Person>(p => p.IsActive));
list.SetSorting(new ExpressionSortingProperty<Person>(p => p.Name));
```

### Fluent builder

```csharp
using var list = new ExtendedCollectionBuilder<Person>()
    .From(people)
    .Where(p => p.IsActive)
    .OrderBy(p => p.Name)
    .Build();
```

When an item implements `INotifyPropertyChanged`, properties referenced in the active filter or sort expressions trigger a pipeline refresh automatically.

### Selection (without row wrappers)

Use `SelectableCollection<T>` on top of an `ExtendedCollection<T>`:

```csharp
using MyNet.Observable.Collections.Selection;

// Owns the extended collection — dispose once
using var selectable = SelectableCollection.From(items, SelectionMode.Multiple);
selectable.Select(items[0]);

// External collection — only disposes selection state
using var collection = ExtendedCollection.From(items);
using var selectable = new SelectableCollection<Item>(collection);
```

For per-row `IsSelected` binding, prefer list items that are `ObservableObject` instances with `SelectionBehavior` rather than a parallel wrapper collection.

### Aggregates (statistics)

Statistics live in `MyNet.Observable.Collections.Statistics` and observe an `ExtendedCollection<T>` without modifying it. Aggregates run on the **filtered** `Items` view. Dispose statistics alongside the collection:

```csharp
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;

using var list = ExtendedCollection.From(orders);
list.SetFilter(new ExpressionFilter<Order>(o => o.IsActive));

using var stats = list.Statistics(o => o.Amount);
// stats.FilteredPercentage, stats.Sum, stats.Average, stats.Min, stats.Max

using var durations = list.Statistics(o => o.Elapsed);
```

Bind list counts with `Count` and `SourceCount` on the collection; bind derived metrics via `Statistics(...)`.

### UI-thread collection notifications

```csharp
using MyNet.Observable.Collections.Extensions;
using MyNet.Utilities.Collections;

var dispatched = new ObservableRangeCollection<Row>().Scheduled(Scheduler.CurrentThread);
```

---

## Relay / synthetic property names

When old/new values are unknown (forwarded names):

```csharp
NotifyPropertyChanged("Wrapper.Name");
```

Prefer `NotifyPropertyChanged(name, before, after)` from setters when values are known.

---

## Analyzer diagnostics

| ID | Severity | Meaning |
|----|----------|---------|
| MNETMETA001 | Error | Missing metadata under `[assembly: EnforceGeneratedMetadata]` |
| MNETOBS001 | Error | Type must be `partial` for `[ObservableProperty]` |
| MNETOBS002 | Error | Must derive from `ObservableObject` |
| MNETOBS003 | Error | Nested types not supported for `[ObservableProperty]` |
| MNETOBS004 | Warning | Setter assigns without `SetProperty` / changing pipeline |

---

## Migration from the legacy API

| Legacy | Current |
|--------|---------|
| `RegisterBehavior(...)` on `ObservableObject` | `Behaviors.Register(...)` |
| `GetBehavior<T>()` on `ObservableObject` | `Behaviors.Get<T>()` |
| `GeneratedPropertyBehaviorRegistry` | Metadata + `MetadataBehaviorApplicator` |
| `MetadataAttributeBootstrapper.Apply` | Generator + lazy `MetadataRegistry.Get` |
| Fody `OnPropertyChanged` weaving | `SetProperty` or `[ObservableProperty]` |
| `owner.Use<TBehavior>()` (Activator) | `Behaviors.Register(new MyBehavior(owner))` |
| `CountStatistics` / `RangeStatistics` on `ObservableCollection` | `collection.Statistics(...)` on `ExtendedCollection` |

---

## Metadata generation

This section describes the **single supported pipeline** for configuring observable metadata.

### Pipeline (authoring → application → runtime)

```
Property attributes on ObservableObject subclasses
        ↓ compile time
MyNet.Observable.Metadata.Generator
        ↓ GeneratedMetadataProviders.g.cs (lazy bootstrap per type)
MetadataRegistry.Get(type) → ObservableMetadataBootstrap.Ensure(type)
        ↓ runtime
MetadataBehaviorApplicator.Apply → behaviors
```

#### What you declare

Put metadata attributes on **properties** of types that derive from `ObservableObject`:

```csharp
using MyNet.Observable;
using MyNet.Observable.Behaviors.Metadata.Attributes;

public sealed class Person : ObservableObject
{
    [UpdateOnCultureChanged]
    public string Name { get; set; } = string.Empty;

    [IgnoreModificationTracking]
    public int InternalId { get; set; }

    [AlsoValidate(nameof(Email))]
    public string ConfirmEmail { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [ForwardProperty]
    public AddressViewModel Address { get; set; } = null!;
}
```

Declare observable properties with `[ObservableProperty]` on a **partial** backing field, or set values through `SetProperty` in explicit setters.

#### What the generator does

When at least one supported attribute is present on a property of a type that **derives from `ObservableObject`**, the generator emits `ObservableMetadataBootstrap` with one `Configure_{TypeName}()` method per type. Configuration runs **lazily** on the first `MetadataRegistry.Get(type)` for that type (no `[ModuleInitializer]`). Each configure method invokes `MetadataApplicators` on the relevant `PropertyMetadata` entries.

Inspect generated code under `obj/` → `GeneratedMetadataProviders.g.cs`.

Types that are not `ObservableObject` descendants are **not** generated (even if they carry metadata attributes).

#### What consumes metadata at runtime

- `MetadataRegistry.Get` — ensures generated configuration for the type, then returns `TypeMetadata`
- Features: `ModificationTrackingFeature`, `EventReactionFeature`, `ValidationDependencyFeature`, `PropertyChangedForwardingFeature`, …
- `MetadataBehaviorApplicator.Apply` — reads `PropertyChangedForwardingFeature` from metadata and registers `PropertyChangedForwardingBehavior` (called from `ObservableObject` constructor)

No application startup call is required.

### Supported metadata attributes

| Attribute | Effect |
|-----------|--------|
| `[IgnoreModificationTracking]` | Property excluded from modification tracking |
| `[UpdateOnCultureChanged]` | Refresh when culture changes |
| `[UpdateOnTimeZoneChanged]` | Refresh when time zone changes |
| `[AlsoValidate("OtherProperty")]` | Validation dependency |
| `[ForwardProperty(concatenatePropertyName: true)]` | Relay child `INotifyPropertyChanged` to owner |

### Strict mode (fail-fast)

Add to any file in the assembly (e.g. `AssemblyInfo.cs`):

```csharp
using MyNet.Observable.Metadata;

[assembly: EnforceGeneratedMetadata]
```

When enabled, types deriving from `ObservableObject` **without** any generated metadata configuration produce compile-time error `MNETMETA001`.

Use `[ExemptFromGeneratedMetadata]` from `MyNet.Observable.Metadata` on types that intentionally have no metadata (abstract bases, markers, infrastructure VMs).

### Manual fluent configuration (secondary)

Use when attributes are not possible (third-party types, dynamic scenarios, tests).

#### Per-property (preferred for manual setup)

```csharp
MetadataRegistry.For<MyType>()
    .Property(x => x.DisplayName)
    .UpdateOnCultureChanged();
```

#### Batch by property name

```csharp
MetadataRegistry.Get(typeof(MyType))
    .UpdateOnCultureChanged(nameof(MyType.DisplayName), nameof(MyType.Subtitle));
```

#### API surfaces (do not duplicate)

| Surface | Role |
|---------|------|
| `MetadataRegistry.For<T>().Property(expr).…` | **Authoring** — type-safe fluent configuration |
| `TypeMetadata.UpdateOnCultureChanged(names…)` | **Batch authoring** on several properties |
| `TypeMetadata.WithFeature<T>()` / `GetFeatureOrDefault<T>(name)` | **Runtime queries** (behaviors, forwarding) |
| `PropertyMetadata.TryGetFeature<T>()` | **Low-level** feature access on a property instance |

Do **not** duplicate the same rules with attributes and manual fluent configuration on the same type.

### Metadata team rules

1. **Author with attributes** on `ObservableObject` properties.
2. **Rely on the metadata generator** for lazy bootstrap (`MetadataRegistry.Get` → `MetadataApplicators`).
3. **Do not** configure the same property via attributes and manual fluent API.
4. Use **`[assembly: EnforceGeneratedMetadata]`** when you want compile-time coverage.
5. Imperative forwarding: `owner.ForwardProperty(...)` or `MetadataBehaviorApplicator.ApplyForwardProperty` — updates metadata and registers the behavior.
6. Strict-mode opt-out: `[ExemptFromGeneratedMetadata]` on types without metadata attributes.

### Metadata benefits

1. **Type-safe** — mapping attribute → feature is generated at compile time
2. **Performance** — no reflection at startup for normal types
3. **Explicit** — attributes document intent on properties
4. **Fail-fast** — optional strict mode (`MNETMETA001`)
5. **Traceable** — generated initializer is readable in `GeneratedMetadataProviders.g.cs`

---

## Related packages

- [UI presentation layer](ui.md)
- [Globalization](globalization.md) — culture services for behaviors
- [Foundations](foundations.md) — `MyNet.Metadata` registry primitives

## Package README

[MyNet.Observable README](../../src/MyNet.Observable/README.md)
