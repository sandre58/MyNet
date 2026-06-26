<div align="center">

# MyNet.Mail

<img src="../../assets/MyNetMail.png" alt="MyNet.Mail" width="96" height="96" />

*Email message and attachment models, address parsing, and SMTP service abstractions independent of a mail transport.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Mail)](https://www.nuget.org/packages/MyNet.Mail)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Email builder** | Fluent API for recipients, subject, and body |
| **Attachments** | Models and address parsing |
| **IMailService** | Transport-independent mail abstraction |
| **MailKit** | Optional SMTP delivery via sibling package |

---

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




---
## Related packages

- [MyNet.Mail.MailKit](https://www.nuget.org/packages/MyNet.Mail.MailKit)
- [MyNet.Platform.Windows](https://www.nuget.org/packages/MyNet.Platform.Windows) — MAPI (Windows)




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
