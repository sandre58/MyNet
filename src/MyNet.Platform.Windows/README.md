<div align="center">

# MyNet.Platform.Windows

<img src="../../assets/MyNetPlatformWindows.png" alt="MyNet.Platform.Windows" width="96" height="96" />

*Windows platform adapters: registry access, authentication integrations, and MAPI mail support for MyNet.IO and MyNet.Utilities.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Platform.Windows)](https://www.nuget.org/packages/MyNet.Platform.Windows)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Registry** | Windows implementations for MyNet.IO |
| **Authentication** | Windows identity integrations |
| **MAPI** | Desktop mail support via native APIs |
| **Adapters** | Platform code kept out of portable packages |

---

## Installation

```bash
dotnet add package MyNet.Platform.Windows
```

Reference only from Windows-targeted executables.

## Quick start

Use Windows-specific implementations from this package when you need registry-backed settings or MAPI mail instead of SMTP. Keep portable logic in **MyNet.IO** and **MyNet.Utilities**.



---
## Related packages

- [MyNet.IO](https://www.nuget.org/packages/MyNet.IO)
- [MyNet.Utilities](https://www.nuget.org/packages/MyNet.Utilities)
- [MyNet.Mail](https://www.nuget.org/packages/MyNet.Mail) — SMTP abstractions (cross-platform)



---
## Documentation

- [IO & platform guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/io-platform.md)
- [Mail guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/mail.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
