<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <img src="../../assets/MyNetUtilities.png" width="128" alt="MyNetUtilities">
</div>

<h1 align="center">My .NET Utilities Geography</h1>

[![MIT License](https://img.shields.io/github/license/sandre58/mynet?style=for-the-badge)](https://github.com/sandre58/mynet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Utilities.Geography.Extensions?style=for-the-badge)](https://www.nuget.org/packages/MyNet.Utilities.Geography.Extensions)

A comprehensive extension library for accessing detailed geography information in .NET applications.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](#)
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-purple)](#)
[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-purple)](#)
[![C#](https://img.shields.io/badge/language-C%23-blue)](#)

---

## Installation

Install via NuGet:

```bash
dotnet add package MyNet.Utilities.Geography.Extensions
```

## Features

- Access country names, codes, flags, and demographics.
- Utilities for working with country data and resources.
- Flag image support and formatting.

## Example Usage

```csharp
using MyNet.Utilities.Geography.Extensions;

// Get country name by code
var name = CountryExtensions.GetName("FR");

// Get flag image
var flag = CountryExtensions.GetFlag("US", FlagSize.Large);
```

## Related Packages

| Package | Description | NuGet |
|---|---|---|
| [**MyNet.Utilities**](../MyNet.Utilities) | Core utilities for .NET development. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities) |
| [**MyNet.Utilities.Generator.Extensions**](../MyNet.Utilities.Generator.Extensions) | Generate random data for .NET apps. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Generator.Extensions) |
| [**MyNet.Utilities.Logging.NLog**](../MyNet.Utilities.Logging.NLog) | Logging integration with NLog. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Logging.NLog) |
| [**MyNet.Utilities.Mail.MailKit**](../MyNet.Utilities.Mail.MailKit) | Email sending with MailKit. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Mail.MailKit) |

## License

Copyright © Stéphane ANDRE.

Distributed under the MIT License. See [LICENSE](../../LICENSE) for details.