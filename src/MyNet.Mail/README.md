# MyNet.Mail

Email message and attachment models, address parsing, and SMTP service abstractions independent of a mail transport.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Mail)](https://www.nuget.org/packages/MyNet.Mail)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Mail
```

## Quick start

```csharp
using MyNet.Mail;

var email = Email.From("sender@example.com", "Sender")
    .To("recipient@example.com")
    .Subject("Hello")
    .Body("Plain text body", isHtml: false);
```

Implement or consume `IMailService` via **MyNet.Mail.MailKit** for SMTP.

## Related packages

- [MyNet.Mail.MailKit](https://www.nuget.org/packages/MyNet.Mail.MailKit)
- [MyNet.Platform.Windows](https://www.nuget.org/packages/MyNet.Platform.Windows) — MAPI (Windows)

## Documentation

- [Mail guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/mail.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
