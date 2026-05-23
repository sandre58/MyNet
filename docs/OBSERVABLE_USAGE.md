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

## 6. Relay / synthetic property names

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

## Further reading

- [src/MyNet.Observable/README.md](../src/MyNet.Observable/README.md)
- [METADATA_GENERATION_GUIDE.md](METADATA_GENERATION_GUIDE.md)
