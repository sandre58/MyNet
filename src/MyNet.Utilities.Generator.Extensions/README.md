<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <img src="../../assets/MyNetUtilities.png" width="128" alt="MyNetUtilities">
</div>

<h1 align="center">My .NET Utilities Generator</h1>

[![MIT License](https://img.shields.io/github/license/sandre58/mynet?style=for-the-badge)](https://github.com/sandre58/mynet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Utilities.Generator.Extensions?style=for-the-badge)](https://www.nuget.org/packages/MyNet.Utilities.Generator.Extensions)

A powerful extension library for generating random data in .NET applications. Useful for testing, simulations, and data-driven scenarios.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](#)
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-purple)](#)
[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-purple)](#)
[![C#](https://img.shields.io/badge/language-C%23-blue)](#)

---

## Installation

Install via NuGet:

```bash
dotnet add package MyNet.Utilities.Generator.Extensions
```

## Features

- Generate random names, addresses, internet data, and more.
- Useful for unit tests, demo data, and simulations.
- Extends the core MyNet.Utilities generator capabilities.

## Example Usage

```csharp
using MyNet.Utilities.Generator.Extensions;

var name = NameGenerator.Generate();
var address = AddressGenerator.Generate();
var email = Internet.GenerateEmail();
```

## Related Packages

| Package | Description | NuGet |
|---|---|---|
| [**MyNet.Utilities**](../MyNet.Utilities) | Core utilities for .NET development. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities) |
| [**MyNet.Utilities.Geography.Extensions**](../MyNet.Utilities.Geography.Extensions) | Access detailed geography info. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Geography.Extensions) |
| [**MyNet.Utilities.Logging.NLog**](../MyNet.Utilities.Logging.NLog) | Logging integration with NLog. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Logging.NLog) |
| [**MyNet.Utilities.Mail.MailKit**](../MyNet.Utilities.Mail.MailKit) | Email sending with MailKit. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Mail.MailKit) |

## License

Copyright © Stéphane ANDRE.

Distributed under the MIT License. See [LICENSE](../../LICENSE) for details.