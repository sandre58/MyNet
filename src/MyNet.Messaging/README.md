<div align="center">

# MyNet.Messaging

<img src="../../assets/MyNetMessaging.png" alt="MyNet.Messaging" width="96" height="96" />

*Weak-reference in-process messenger for loosely coupled communication between view models and services (MVVM-friendly).*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Messaging)](https://www.nuget.org/packages/MyNet.Messaging)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Weak references** | Messenger that avoids view model leaks |
| **Messaging** | Register, send, and unregister patterns |
| **Decoupling** | Loose communication without a service locator |
| **Standalone** | No required MyNet package dependencies |

---

## Installation

```bash
dotnet add package MyNet.Messaging
```

## Quick start

```csharp
using MyNet.Messaging;

public sealed class OrderViewModel
{
    public OrderViewModel()
    {
        Messenger.Default.Register<OrderPlacedMessage>(this, msg => OnOrderPlaced(msg));
    }

    private void OnOrderPlaced(OrderPlacedMessage msg) { }
}

Messenger.Default.Send(new OrderPlacedMessage(orderId: 42));
```

Unregister with `Messenger.Default.Unregister(this)` when the subscriber is disposed.




---
## Related packages

Standalone — no required MyNet dependencies.




---
## Documentation

- [Collections & messaging guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/collections-messaging.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
