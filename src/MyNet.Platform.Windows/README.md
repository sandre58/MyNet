# MyNet.Platform.Windows

Windows platform adapters: registry access, authentication integrations, and MAPI mail support.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Platform.Windows)](https://www.nuget.org/packages/MyNet.Platform.Windows)

**Target framework:** .NET 10 · **OS:** Windows

## Installation

```bash
dotnet add package MyNet.Platform.Windows
```

Reference only from Windows-targeted executables.

## Quick start

Use Windows-specific implementations from this package when you need registry-backed settings or MAPI mail instead of SMTP. Keep portable logic in **MyNet.IO** and **MyNet.Utilities**.

## Related packages

- [MyNet.IO](https://www.nuget.org/packages/MyNet.IO)
- [MyNet.Utilities](https://www.nuget.org/packages/MyNet.Utilities)
- [MyNet.Mail](https://www.nuget.org/packages/MyNet.Mail) — SMTP abstractions (cross-platform)

## Documentation

- [IO & platform guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/io-platform.md)
- [Mail guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/mail.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
