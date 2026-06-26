<div align="center">

# MyNet.IO

<img src="../../assets/MyNetIO.png" alt="MyNet.IO" width="96" height="96" />

*File and path helpers, auto-save coordinators, and portable registry abstractions for desktop and cross-platform apps.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.IO)](https://www.nuget.org/packages/MyNet.IO)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Files** | Path and file extension helpers |
| **Auto-save** | Coordinators for documents and settings |
| **Registry** | Portable abstractions for desktop apps |
| **UI integration** | Workspace and recent-file scenarios |

---

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




---
## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Platform.Windows](https://www.nuget.org/packages/MyNet.Platform.Windows)
- [MyNet.UI](https://www.nuget.org/packages/MyNet.UI)




---
## Documentation

- [IO & platform guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/io-platform.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
