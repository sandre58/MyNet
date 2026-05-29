# Mail

SMTP email abstractions and MailKit transport.

## Packages

| Package | Role |
|---------|------|
| [MyNet.Mail](../../src/MyNet.Mail/README.md) | `Email`, attachments, `IMailService`, `SmtpClientOptions` |
| [MyNet.Mail.MailKit](../../src/MyNet.Mail.MailKit/README.md) | MailKit SMTP + `AddMailKitMailService()` |

## Building messages

```csharp
using MyNet.Mail;

var email = Email.From("sender@example.com", "Sender Name")
    .To("recipient@example.com", "Recipient")
    .Cc("cc@example.com")
    .Subject("Hello from MyNet")
    .Body("<p>HTML body</p>", isHtml: true)
    .Attach("report.pdf", fileBytes, "application/pdf");
```

Fluent API returns `Email` instances consumed by `IMailService`.

## DI with MailKit

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Mail;
using MyNet.Mail.MailKit;
using MyNet.Mail.Smtp;

services.AddMailKitMailService();

var factory = provider.GetRequiredService<IMailServiceFactory>();
await using var mail = factory.Create(new SmtpClientOptions
{
    Server = "smtp.example.com",
    Port = 587,
    UseSsl = true,
    RequiresAuthentication = true,
    User = "user@example.com",
    Password = "secret",
    PreferredEncoding = "utf-8",
});
```

### Send

```csharp
var response = await mail.SendAsync(email, cancellationToken);

if (!response.Successful)
{
    foreach (var error in response.ErrorMessages)
        logger.LogError("{Error}", error);
}
else
{
    var messageId = response.MessageId;
}
```

Dispose `IMailService` when you manage lifetime (`IAsyncDisposable` / `IDisposable`) to reuse SMTP connections correctly.

## TLS / ports

| Port | Typical setup |
|------|----------------|
| **587** | STARTTLS — `UseSsl = true`, `SecurityMode.Auto` |
| **465** | Implicit SSL on connect |
| **25** | Often blocked; dev only |

`SmtpSecurityMode` on `SmtpClientOptions`: `Auto`, `SslOnConnect`, `StartTls`, etc.

## Pickup directory (IIS / local dev)

```csharp
var options = new SmtpClientOptions
{
    UsePickupDirectory = true,
    MailPickupDirectory = @"C:\inetpub\mailroot\Pickup",
};
```

## Without MailKit

Use **MyNet.Mail** models and `IMailService` from your own transport, or **MyNet.Platform.Windows** for MAPI on Windows desktops.

## Testing

Mock `IMailService` or use pickup directory options. See `tests/MyNet.Mail.Tests`, `tests/MyNet.Mail.MailKit.Tests`.

## Related

- [IO & platform](io-platform.md) — MAPI on Windows
- [Utilities](foundations.md#mynetutilities) — logging helpers
