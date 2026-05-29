# MyNet.Collections

Observable collections with range notifications, keyed collections, synchronizers, and batch update helpers for MVVM.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Collections)](https://www.nuget.org/packages/MyNet.Collections)

**Target framework:** .NET 10

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

## Related packages

- [MyNet.Observable](https://www.nuget.org/packages/MyNet.Observable) — extended/selectable collection wrappers
- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)

## Documentation

- [Collections & messaging guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/collections-messaging.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
