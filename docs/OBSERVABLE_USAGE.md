# MyNet.Observable — Consumer guide

This guide describes how to use **MyNet.Observable** after the behavior/metadata refactor. For metadata internals, see [METADATA_GENERATION_GUIDE.md](METADATA_GENERATION_GUIDE.md).

## Installation

```bash
dotnet add package MyNet.Observable
```

The package references the **MyNet.Observable.Metadata.Generator** analyzer (metadata bootstrap, `[ObservableProperty]`, setter usage diagnostic).

## 1. Observable properties

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

## 2. Behaviors (composition)

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

## 3. Property metadata (attributes)

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

## 4. Strict metadata mode (optional)

```csharp
using MyNet.Observable.Metadata;

[assembly: EnforceGeneratedMetadata]
```

Types deriving from `ObservableObject` without metadata produce **MNETMETA001**. Opt out with `[ExemptFromGeneratedMetadata]` on intentional bare types.

## 5. Notification suspension

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

## 6. Extended collections (filter, sort, group, selection)

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

## 7. Relay / synthetic property names

When old/new values are unknown (forwarded names):

```csharp
NotifyPropertyChanged("Wrapper.Name");
```

Prefer `NotifyPropertyChanged(name, before, after)` from setters when values are known.

## Analyzer diagnostics

| ID | Severity | Meaning |
|----|----------|---------|
| MNETMETA001 | Error | Missing metadata under `[assembly: EnforceGeneratedMetadata]` |
| MNETOBS001 | Error | Type must be `partial` for `[ObservableProperty]` |
| MNETOBS002 | Error | Must derive from `ObservableObject` |
| MNETOBS003 | Error | Nested types not supported for `[ObservableProperty]` |
| MNETOBS004 | Warning | Setter assigns without `SetProperty` / changing pipeline |

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

## Further reading

- [src/MyNet.Observable/README.md](../src/MyNet.Observable/README.md)
- [METADATA_GENERATION_GUIDE.md](METADATA_GENERATION_GUIDE.md)
