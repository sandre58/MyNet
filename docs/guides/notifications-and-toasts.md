# Notifications & toasts

**Package:** [MyNet.UI](../../src/MyNet.UI/README.md)

In-app **notification center** and **toast** overlays driven by a shared publish pipeline. There is no `INotificationPresenter` / `IToastPresenter` — bind your UI to observable collections on the managers.

## Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Notifications;
using MyNet.UI.Toasting;

services.AddNotifications(configureProcessors: processors =>
{
    // processors.Add(new MyNotificationProcessor());
});

services.AddToasting(options =>
{
    options.MaxVisibleToasts = 5;
    // options.DefaultDuration = TimeSpan.FromSeconds(4);
});
```

| Extension | Registers |
|-----------|-----------|
| `AddNotifications()` | `INotificationService`, `INotificationsManager`, scheduler |
| `AddToasting()` | `IToastManager`, `IToastFactory`, `IToastFilter`, own `INotificationService` |

**Note:** `AddToasting()` registers a **separate** `INotificationService` with an empty processor list. For one publish stream feeding both the notification drawer and toasts, publish through the service instance that your processors and `ToastManager` share — typically register both and inject `INotificationsManager` + `IToastManager`, and publish via the notification service resolved after both extensions (verify in your host; see `NotificationsToastingRegistrationTests`).

Recommended host order:

```csharp
services.AddNotifications();
services.AddToasting();
services.AddShell(); // wires shell notification drawer to INotificationsManager
```

## Notifications

### Publish

```csharp
using MyNet.UI.Notifications;

public class ExportService(INotificationPublisher notifications)
{
    public async Task ExportAsync()
    {
        notifications.Publish(new MessageNotification(
            "Export completed",
            severity: NotificationSeverity.Success));
    }
}
```

`INotificationService` combines `INotificationPublisher` + `INotificationStream`.

### Display in UI

Bind to the manager collection (WPF example):

```xml
<!-- ItemsSource = notificationsManager.Notifications -->
```

```csharp
public MainViewModel(INotificationsManager manager)
{
    Notifications = manager.Notifications; // ReadOnlyObservableCollection<INotification>
}
```

### Processors

Implement `INotificationProcessor` to transform or filter before items appear:

```csharp
public sealed class DeduplicateProcessor : INotificationProcessor
{
    public bool TryPublish(INotification notification, out INotification? processed)
    {
        // return false to drop; or replace notification
        processed = notification;
        return true;
    }
}
```

Register via `AddNotifications(processors => processors.Add(new DeduplicateProcessor()))`.

## Toasts

`ToastManager` listens to `INotificationService` and projects matching notifications into `IToastManager.Toasts`.

```csharp
using MyNet.UI.Toasting;

public class ToastHostViewModel(IToastManager toastManager)
{
    public ReadOnlyObservableCollection<IToast> Toasts => toastManager.Toasts;
}
```

Customize filtering with `IToastFilter` (default: `AllToastsFilter`). Customize creation with `IToastFactory` (default: `DefaultToastFactory`).

### Direct test pattern

```csharp
using var notificationService = new NotificationService();
using var toastManager = new ToastManager(
    notificationService,
    scheduler,
    factory,
    new AllToastsFilter(),
    new ToastManagerOptions());

notificationService.Publish(new MessageNotification("Hello"));
// toastManager.Toasts should contain one item
```

See `tests/MyNet.UI.Tests/Toasting/ToastManagerTests.cs`.

## Shell integration

`AddShell()` registers `ShellNotificationsViewModel` and `ShellNotificationsDrawerCoordinator`, which connect the notification drawer to `INotificationsManager`. See [Shell guide](shell.md).

## Severity and types

Use concrete notification types from `MyNet.UI.Notifications` (e.g. `MessageNotification`). Severity drives styling in the host (`NotificationSeverity`).

## Related

- [Shell](shell.md)
- [Dialogs](dialogs.md)
- [Globalization](globalization.md) — localized notification text via `ITranslationService`
