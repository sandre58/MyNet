# MyNet.Messaging

Weak-reference in-process messenger for loosely coupled communication between view models and services (MVVM-friendly).

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Messaging)](https://www.nuget.org/packages/MyNet.Messaging)

**Target framework:** .NET 10

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

## Related packages

Standalone — no required MyNet dependencies.

## Documentation

- [Collections & messaging guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/collections-messaging.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
