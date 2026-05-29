# MyNet.Observable

MVVM-oriented observable models with INotifyPropertyChanged, edition tracking, FluentValidation, metadata, and an included Roslyn source generator.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Observable)](https://www.nuget.org/packages/MyNet.Observable)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Observable
```

Includes the **MyNet.Observable.Metadata.Generator** analyzer (not a separate NuGet package).

## Quick start

```csharp
using MyNet.Observable;
using MyNet.Observable.Metadata;

public partial class PersonViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
}
```

Behaviors (optional):

```csharp
public PersonViewModel()
{
    this.UseTracking();
    this.UseValidation(new PersonValidator());
}
```

## Related packages

- [MyNet.UI](https://www.nuget.org/packages/MyNet.UI)
- [MyNet.Globalization](https://www.nuget.org/packages/MyNet.Globalization)
- [MyNet.Collections](https://www.nuget.org/packages/MyNet.Collections)

## Documentation

- [Observable models guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/observable.md) (consumer usage + metadata generation)
- [Documentation index](https://github.com/sandre58/MyNet2/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet2/blob/main/LICENSE).
