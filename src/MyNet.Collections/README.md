<div align="center">

# MyNet.Collections

<img src="../../assets/MyNetCollections.png" alt="MyNet.Collections" width="96" height="96" />

*Observable collections with range notifications, keyed collections, synchronizers, and batch update helpers for MVVM.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Collections)](https://www.nuget.org/packages/MyNet.Collections)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Range collection** | Batch AddRange and ReplaceRange with notifications |
| **Keyed patterns** | Selectable and keyed collection helpers |
| **Synchronizers** | Multi-source collection sync for MVVM |
| **Notifications** | Efficient range change events for UI binding |

---

## Installation

```bash
dotnet add package MyNet.Collections
```

## Quick start

```csharp
using MyNet.Collections;

var items = new ObservableRangeCollection<string>();
items.AddRange(["alpha", "beta", "gamma"]);
items.ReplaceRange(0, 1, ["A"]);
```




---
## Related packages

- [MyNet.Observable](https://www.nuget.org/packages/MyNet.Observable) — extended/selectable collection wrappers
- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)




---
## Documentation

- [Collections & messaging guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/collections-messaging.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
