<div align="center">

# MyNet.Observable

<img src="MyNetObservable.png" alt="MyNet.Observable" width="96" height="96" />

*MVVM-oriented observable models with INotifyPropertyChanged, edition tracking, FluentValidation, metadata, and an included Roslyn source generator.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Observable)](https://www.nuget.org/packages/MyNet.Observable)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **ObservableObject** | INotifyPropertyChanged base with change notification |
| **Behaviors** | Edition tracking and FluentValidation integration |
| **Source generator** | Roslyn codegen for observable properties |
| **Metadata** | Labels, groups, and UI hints for bound models |

---

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



---
## Related packages

- [MyNet.UI](https://www.nuget.org/packages/MyNet.UI)
- [MyNet.Globalization](https://www.nuget.org/packages/MyNet.Globalization)
- [MyNet.Collections](https://www.nuget.org/packages/MyNet.Collections)



---
## Documentation

- [Observable models guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/observable.md) (consumer usage + metadata generation)
- [Documentation index](https://github.com/sandre58/MyNet2/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
