<div align="center">

# MyNet.Mail.MailKit

<img src="../../assets/MyNetMail.png" alt="MyNet.Mail.MailKit" width="96" height="96" />

*MailKit implementation of MyNet.Mail IMailService with TLS configuration, connection reuse, and Microsoft.Extensions.DependencyInjection registration.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Mail.MailKit)](https://www.nuget.org/packages/MyNet.Mail.MailKit)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **MailKit** | IMailService implementation for SMTP |
| **TLS** | Configuration and connection reuse |
| **DI** | AddMailKit() registration extensions |
| **Production** | Ready transport for MyNet.Mail models |

---

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



---
## Related packages

- [MyNet.Mail](https://www.nuget.org/packages/MyNet.Mail)
- [MyNet.Utilities](https://www.nuget.org/packages/MyNet.Utilities)



---
## Documentation

- [Mail guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/mail.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
