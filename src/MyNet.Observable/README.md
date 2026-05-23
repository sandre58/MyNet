# MyNet.Observable

Base library for **observable, editable, and validatable** models in .NET (MVVM-friendly).

## Installation

```bash
dotnet add package MyNet.Observable
```

The package includes the **MyNet.Observable.Metadata.Generator** analyzer (metadata bootstrap + `[ObservableProperty]` + usage diagnostics).

## Quick start

### 1. Observable properties

Prefer a **partial** type and `[ObservableProperty]` on backing fields:

```csharp
using MyNet.Observable;
using MyNet.Observable.Metadata;

public partial class PersonViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
}
```

Or use `SetProperty` in explicit setters:

```csharp
public string Name
{
    get => _name;
    set => SetProperty(ref _name, value);
}
```

`SetProperty` runs the full **changing → assign → changed** pipeline, honors `Cancel` from behaviors, and supports notification suspension.

### 2. Behaviors (composition)

Register optional capabilities on `Behaviors`:

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

| Extension | Behavior |
|-----------|----------|
| `UseTracking()` | Dirty / modification tracking |
| `UseValidation(validator)` | FluentValidation + `INotifyDataErrorInfo` |
| `ReactOnCultureChanged(service)` | Refresh culture-sensitive properties |
| `ReactOnTimeZoneChanged(service)` | Refresh time-zone-sensitive properties |
| `ForwardProperty(x => x.Child)` | Relay child `INotifyPropertyChanged` |

Access behaviors: `vm.Behaviors.Get<T>()`, `TryGet`, `GetAll`, `Unregister<T>()`.

### 3. Property metadata (attributes)

Declare metadata on **properties** of `ObservableObject` subclasses:

```csharp
using MyNet.Observable;
using MyNet.Observable.Behaviors.Metadata.Attributes;

public sealed class PersonViewModel : ObservableObject
{
    [UpdateOnCultureChanged]
    public string DisplayName { get; set; } = string.Empty;

    [IgnoreModificationTracking]
    public int InternalId { get; set; }

    [ForwardProperty]
    public AddressViewModel Address { get; set; } = null!;
}
```

At compile time, **MyNet.Observable.Metadata.Generator** emits lazy bootstrap code. The first `MetadataRegistry.Get(typeof(YourType))` applies `MetadataApplicators` for that type. `ObservableObject` applies forwarding behaviors from metadata in its constructor.

See [Metadata generation guide](../../docs/METADATA_GENERATION_GUIDE.md) for the full pipeline.

### 4. Strict metadata mode (optional)

In `AssemblyInfo.cs` or any project file:

```csharp
using MyNet.Observable.Metadata;

[assembly: EnforceGeneratedMetadata]
```

Types deriving from `ObservableObject` without metadata attributes produce **MNETMETA001**. Opt out with `[ExemptFromGeneratedMetadata]` on intentional bare types (bases, markers).

## Notification suspension

```csharp
using (SuspendNotifications()) // CoalesceOnResume (default)
{
    Name = "a";
    Name = "b";
} // one PropertyChanged for Name (first old → last new)

using (SuspendNotifications(NotificationSuspensionMode.Drop))
{
    Name = "x"; // no notifications, not replayed
}
```

## Analyzer diagnostics

| ID | Severity | Meaning |
|----|----------|---------|
| MNETMETA001 | Error | `ObservableObject` type missing metadata under strict mode |
| MNETOBS001 | Error | Host type must be `partial` for `[ObservableProperty]` |
| MNETOBS002 | Error | Host must derive from `ObservableObject` |
| MNETOBS003 | Error | Nested types not supported for `[ObservableProperty]` |
| MNETOBS004 | Warning | Setter assigns without `SetProperty` / changing pipeline |

## Relay / synthetic property names

When old/new values are unknown (e.g. forwarded `Wrapper.Name`), call:

```csharp
NotifyPropertyChanged("Wrapper.Name");
```

Prefer `NotifyPropertyChanged(name, before, after)` from setters when values are known.

## Further reading

- [Consumer guide (Observable usage)](../../docs/OBSERVABLE_USAGE.md)
- [Metadata generation guide](../../docs/METADATA_GENERATION_GUIDE.md)
- Architecture notes in [`.ai/architecture.md`](../../.ai/architecture.md)

## License

MIT — see [LICENSE](../../LICENSE).
