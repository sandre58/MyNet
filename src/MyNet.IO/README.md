# MyNet.IO

File and path helpers, auto-save coordinators, and portable registry abstractions for desktop and cross-platform apps.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.IO)](https://www.nuget.org/packages/MyNet.IO)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.IO
```

## Quick start

```csharp
using MyNet.IO;

FileHelper.EnsureDirectoryExists(@"C:\Data\exports");
```

Use `MyNet.IO` namespaces for path extensions, file helpers, and auto-save patterns. **MyNet.UI** consumes IO abstractions for workspace and recent-file scenarios.

## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Platform.Windows](https://www.nuget.org/packages/MyNet.Platform.Windows)
- [MyNet.UI](https://www.nuget.org/packages/MyNet.UI)

## Documentation

- [IO & platform guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/io-platform.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
