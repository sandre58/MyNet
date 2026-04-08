<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <img src="../../assets/MyNetUtilities.png" width="128" alt="MyNetUtilities">
</div>

<h1 align="center">My .NET Utilities MailKit</h1>

[![MIT License](https://img.shields.io/github/license/sandre58/mynet?style=for-the-badge)](https://github.com/sandre58/mynet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Utilities.Mail.MailKit?style=for-the-badge)](https://www.nuget.org/packages/MyNet.Utilities.Mail.MailKit)

A powerful extension library for sending emails in .NET applications using MailKit.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](#)
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-purple)](#)
[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-purple)](#)
[![C#](https://img.shields.io/badge/language-C%23-blue)](#)

---

## Installation

Install via NuGet:

```bash
dotnet add package MyNet.Utilities.Mail.MailKit
```

## Features

- Send emails using MailKit.
- Support for multiple sender addresses and server configurations.
- Exception handling for undefined servers and empty sender addresses.

## Example Usage

```csharp
using MyNet.Utilities.Mail.MailKit;

// Create a mail service
var mailService = MailKitServiceFactory.Create();

// Send an email
await mailService.SendAsync(
    to: "recipient@example.com",
    subject: "Hello from MyNet",
    body: "This is a test email."
);
```

## Related Packages

| Package | Description | NuGet |
|---|---|---|
| [**MyNet.Utilities**](../MyNet.Utilities) | Core utilities for .NET development. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities) |
| [**MyNet.Utilities.Generator.Extensions**](../MyNet.Utilities.Generator.Extensions) | Generate random data for .NET apps. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Generator.Extensions) |
| [**MyNet.Utilities.Localization.Extensions**](../MyNet.Utilities.Localization.Extensions) | Localization resources and helpers. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Localization.Extensions) |
| [**MyNet.Utilities.Logging.NLog**](../MyNet.Utilities.Logging.NLog) | Logging integration with NLog. | [NuGet](https://www.nuget.org/packages/MyNet.Utilities.Logging.NLog) |

## License

Copyright © Stéphane ANDRE.

Distributed under the MIT License. See [LICENSE](../../LICENSE) for details.