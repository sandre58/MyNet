<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <img src="../../assets/MyNetUtilities.png" width="128" alt="MyNetUtilities">
</div>

<h1 align="center">My .NET Mail — MailKit</h1>

[![MIT License](https://img.shields.io/github/license/sandre58/mynet?style=for-the-badge)](https://github.com/sandre58/mynet/blob/main/LICENSE)

Cross-platform SMTP email sending for .NET via [MailKit](https://github.com/jstedfast/MailKit), implementing `IMailService` from **MyNet.Mail**.

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-purple)](#)
[![C#](https://img.shields.io/badge/language-C%23-blue)](#)

---

## Installation

```bash
dotnet add package MyNet.Mail.MailKit
```

Project reference:

```xml
<ProjectReference Include="path/to/MyNet.Mail.MailKit.csproj" />
```

---

## Dependency injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Mail;
using MyNet.Mail.MailKit;
using MyNet.Mail.Smtp;

services.AddMailKitMailService();

// Later:
var factory = serviceProvider.GetRequiredService<IMailServiceFactory>();
var mail = factory.Create(new SmtpClientOptions
{
    Server = "smtp.example.com",
    Port = 587,
    UseSsl = true,
    RequiresAuthentication = true,
    User = "user",
    Password = "secret",
    PreferredEncoding = "utf-8",
});

var email = Email.From("sender@example.com", "Sender")
    .To("recipient@example.com")
    .Subject("Hello")
    .Body("<p>Hello from MyNet.</p>", isHtml: true);

var response = await mail.SendAsync(email);
if (!response.Successful)
{
    // response.ErrorMessages
}
// response.MessageId when sent over SMTP
```

Dispose the service when you manage its lifetime (it reuses the SMTP connection):

```csharp
if (mail is IDisposable disposable)
{
    disposable.Dispose();
}
```

---

## TLS / security

| Scenario | Configuration |
|---|---|
| Port **587** + TLS | `Port = 587`, `UseSsl = true` (STARTTLS required) |
| Port **465** (implicit TLS) | `Port = 465` — `SecurityMode.Auto` selects SSL on connect |
| Explicit mode | `SecurityMode = SmtpSecurityMode.SslOnConnect` (or `StartTls`, etc.) |

`SecurityMode` is defined on `SmtpClientOptions` in **MyNet.Mail**.

---

## Pickup directory (IIS / local dev)

```csharp
var options = new SmtpClientOptions
{
    UsePickupDirectory = true,
    MailPickupDirectory = @"C:\inetpub\mailroot\Pickup",
};
```

---

## Related packages

| Package | Description |
|---|---|
| [**MyNet.Mail**](../MyNet.Mail) | Models, `IMailService`, `SmtpClientOptions` |
| [**MyNet.Utilities**](../MyNet.Utilities) | Logging helpers |

## License

Copyright © Stéphane ANDRE. MIT — see [LICENSE](../../LICENSE).
