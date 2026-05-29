# MyNet.Mail.MailKit

MailKit SMTP implementation of `IMailService` with TLS options, connection reuse, and dependency injection support.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Mail.MailKit)](https://www.nuget.org/packages/MyNet.Mail.MailKit)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Mail.MailKit
```

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Mail;
using MyNet.Mail.MailKit;
using MyNet.Mail.Smtp;

var services = new ServiceCollection();
services.AddMailKitMailService();

// After BuildServiceProvider():
var factory = serviceProvider.GetRequiredService<IMailServiceFactory>();
await using var mail = factory.Create(new SmtpClientOptions
{
    Server = "smtp.example.com",
    Port = 587,
    UseSsl = true,
    RequiresAuthentication = true,
    User = "user",
    Password = "secret",
});

var email = Email.From("sender@example.com", "Sender")
    .To("recipient@example.com")
    .Subject("Hello")
    .Body("<p>Hello from MyNet.</p>", isHtml: true);

var response = await mail.SendAsync(email);
```

## Related packages

- [MyNet.Mail](https://www.nuget.org/packages/MyNet.Mail)
- [MyNet.Utilities](https://www.nuget.org/packages/MyNet.Utilities)

## Documentation

- [Mail guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/mail.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
