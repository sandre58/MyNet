<div align="center">

# MyNet.Utilities

<img src="../../assets/MyNetUtilities.png" alt="MyNet.Utilities" width="96" height="96" />

*Cross-cutting utilities: caching, encryption, authentication helpers, progress reporting, threading, deferral, and DI-friendly services.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Utilities)](https://www.nuget.org/packages/MyNet.Utilities)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Caching** | In-memory storage with absolute and sliding expiration |
| **Security** | AES encryption and authentication helpers |
| **Progress** | Reporting, debouncing, and serialized async runners |
| **Threading** | Deferral, suspending scopes, and coordination |

---

## Installation

```bash
dotnet add package MyNet.Utilities
```

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Utilities.Cache; // register cache services in your host

var services = new ServiceCollection();
// services.AddMemoryCache(); etc. — see package extensions and tests
```

Explore namespaces under `MyNet.Utilities` for cache, encryption, progress, and threading helpers.



---
## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)



---
## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
