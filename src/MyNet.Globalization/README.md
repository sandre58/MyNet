<div align="center">

# MyNet.Globalization

<img src="../../assets/MyNetGlobalization.png" alt="MyNet.Globalization" width="96" height="96" />

*Culture-aware formatting, localization services, inflection, and translation resource registration for DI-based applications.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Globalization)](https://www.nuget.org/packages/MyNet.Globalization)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Formatting** | Culture-aware formatting and localization |
| **Translations** | Resource registration for DI hosts |
| **Inflection** | Grammatically correct UI text helpers |
| **DI** | Microsoft.Extensions.DependencyInjection integration |

---

## Installation

```bash
dotnet add package MyNet.Globalization
```

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization.Extensions;

var services = new ServiceCollection();
services.AddGlobalization();
services.AddLocalization();
services.AddInflection();
```



---
## Related packages

- [MyNet.Text](https://www.nuget.org/packages/MyNet.Text)
- [MyNet.Humanizer](https://www.nuget.org/packages/MyNet.Humanizer)
- [MyNet.Observable](https://www.nuget.org/packages/MyNet.Observable)



---
## Documentation

- [Globalization guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/globalization.md) (includes translation system)
- [Documentation index](https://github.com/sandre58/MyNet2/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
