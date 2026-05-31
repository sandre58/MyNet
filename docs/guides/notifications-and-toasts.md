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

## Client implementation (toast overlay control)

There is no `IToastPresenter`. The host project provides a **toast overlay control** that binds to `IToastManager.Toasts` — the same pattern as the notification drawer, but typically rendered as a non-modal overlay (top-right corner, above shell content).

```text
INotificationPublisher.Publish(...)
    → INotificationService stream
        → ToastManager (filter + factory + queue)
            → IToastManager.Toasts  ← bind your UI here
```

| Responsibility | Owner |
|----------------|-------|
| Publish notifications | Application services (`INotificationPublisher`) |
| Filter, queue, auto-dismiss timer | `ToastManager` (library) |
| Render toast visuals, animations, placement | **Host UI control** (WPF / Avalonia / …) |
| Close / click commands | `IToast.CloseCommand`, `IToast.ClickCommand` (from factory) |

Register **`AddNotifications()` before `AddToasting()`** so both the notification drawer and `ToastManager` share the same `INotificationService` instance (`TryAddSingleton` keeps the first registration).

### 1. Host view model

Expose the manager collection to the view:

```csharp
using System.Collections.ObjectModel;
using MyNet.UI.Toasting;
using MyNet.UI.Toasting.Models;

public sealed class ToastHostViewModel(IToastManager toastManager) : ObservableObject
{
    public ReadOnlyObservableCollection<IToast> Toasts => toastManager.Toasts;

    public void Dismiss(IToast toast) => toastManager.Remove(toast);
}
```

Register as singleton (one overlay for the whole app):

```csharp
services.AddNotifications();
services.AddToasting(options => options.MaxVisibleToasts = 4);
services.AddSingleton<ToastHostViewModel>();
```

### 2. WPF overlay host

Place a `ToastHost` control in your main window **above** shell content (last child in a root `Grid`, or `Panel.ZIndex`):

```xml
<Grid>
    <!-- Shell content -->
    <ContentControl Content="{Binding ShellContent}" />

    <!-- Toast overlay — does not block shell input outside toast items -->
    <controls:ToastHost DataContext="{Binding ToastHost}"
                        HorizontalAlignment="Right"
                        VerticalAlignment="Top"
                        Margin="16"
                        IsHitTestVisible="True" />
</Grid>
```

`ToastHost.xaml` — stack visible toasts:

```xml
<UserControl x:Class="YourApp.Controls.ToastHost"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ItemsControl ItemsSource="{Binding Toasts}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <StackPanel />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <controls:ToastItem Margin="0,0,0,8" />
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</UserControl>
```

### 3. Single toast item control

Bind to `IToast`. Display text from `Notification`; style from `Notification.Severity`:

```xml
<UserControl x:Class="YourApp.Controls.ToastItem"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:models="clr-namespace:MyNet.UI.Notifications.Models;assembly=MyNet.UI">
    <Border Padding="12,10" CornerRadius="6" MinWidth="280" MaxWidth="420"
            Background="{DynamicResource ToastBackgroundBrush}">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>

            <StackPanel Grid.Column="0">
                <TextBlock Text="{Binding Notification.Title}"
                           FontWeight="SemiBold"
                           Visibility="{Binding Notification.Title, Converter={StaticResource NullOrEmptyToCollapsed}}" />
                <TextBlock Text="{Binding Notification.Message}" TextWrapping="Wrap" />
            </StackPanel>

            <Button Grid.Column="1"
                    Content="×"
                    Command="{Binding CloseCommand}"
                    Visibility="{Binding Settings.ClosingStrategy, Converter={StaticResource ToastCloseVisibility}}" />
        </Grid>

        <Border.InputBindings>
            <MouseBinding MouseAction="LeftClick" Command="{Binding ClickCommand}" />
        </Border.InputBindings>
    </Border>
</UserControl>
```

Map `NotificationSeverity` to brushes in your theme ([Theming](theming.md)) — e.g. success green, error red, warning amber. A `DataTrigger` on `Notification.Severity` or a small converter in the host works well.

**Close button:** `DefaultToastFactory` sets `CloseCommand` only for `IClosableNotification` with `IsClosable == true`. `MessageNotification` has no close command — rely on auto-dismiss, or call `IToastManager.Remove(toast)` from a custom button.

**Click action:** the default factory does not wire `ClickCommand` for `ActionNotification`. Use a custom `IToastFactory` (register **before** `AddToasting()`):

```csharp
using MyNet.UI.Commands;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Models;
using MyNet.UI.Toasting.Settings;

public sealed class AppToastFactory(ICommandFactory commands) : IToastFactory
{
    public IToast Create(INotification notification)
    {
        var settings = new ToastSettings
        {
            ClosingStrategy = ToastClosingStrategy.Both,
            FreezeOnMouseEnter = true
        };

        ICommand? close = notification is IClosableNotification { IsClosable: true } closable
            ? commands.Create((Action)(() => closable.RequestClose()))
            : null;

        ICommand? click = notification is ActionNotification { Action: { } action }
            ? commands.Create((Action)(() => action(notification)))
            : null;

        return new Toast(notification, settings, click, close);
    }
}
```

```csharp
services.AddNotifications();
services.AddSingleton<IToastFactory, AppToastFactory>();
services.AddToasting();
```

### 4. Auto-dismiss and hover pause

`ToastManager` starts an auto-close timer when `ClosingStrategy` is `AutoClose` or `Both`. Duration comes from `toast.Settings.Duration` or `ToastManagerOptions.DefaultDuration`.

`ToastSettings.FreezeOnMouseEnter` is a **host hint** — the library does not pause the timer yet. In your `ToastItem` code-behind or behavior, pause/resume dismissal on pointer enter/leave if you need hover-to-read behavior (track remaining time locally, or call `toastManager.Remove` after a deferred timer).

### 5. Avalonia (sketch)

Same binding model:

```xml
<ItemsControl ItemsSource="{Binding Toasts}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border Classes="toast" Classes.success="{Binding Notification.Severity, Converter=...}">
                <TextBlock Text="{Binding Notification.Message}" />
                <Button Command="{Binding CloseCommand}" Content="×" />
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

Host the control in a top-layer panel (`Panel`, `OverlayLayer`, or `Adorner`) so toasts sit above the shell.

### 6. End-to-end checklist

| Step | Action |
|------|--------|
| DI | `AddNotifications()` then `AddToasting()`; optional custom `IToastFactory` / `IToastFilter` **before** `AddToasting()` |
| View model | `ToastHostViewModel` exposes `IToastManager.Toasts` |
| Overlay | Non-modal panel in main window, top layer |
| Item template | Title, message, severity styling, `CloseCommand`, optional `ClickCommand` |
| Theme | Severity brushes in host resources ([Theming](theming.md)) |
| Publish | `INotificationPublisher.Publish(...)` from app code — no direct UI calls |

### 7. Filtering which notifications become toasts

Not every published notification needs a toast (the drawer may show all, toasts only high-signal items):

```csharp
public sealed class SeverityToastFilter : IToastFilter
{
    public bool ShouldDisplay(INotification notification) =>
        notification.Severity is NotificationSeverity.Warning or NotificationSeverity.Error;
}
```

```csharp
services.AddSingleton<IToastFilter, SeverityToastFilter>();
services.AddToasting();
```

Use `CompositeToastFilter` to combine rules (see `tests/MyNet.UI.Tests/Toasting/CompositeToastFilterTests.cs`).

## Shell integration

`AddShell()` registers `ShellNotificationsViewModel` and `ShellNotificationsDrawerCoordinator`, which connect the notification drawer to `INotificationsManager`. See [Shell guide](shell.md).

## Severity and types

Use concrete notification types from `MyNet.UI.Notifications` (e.g. `MessageNotification`). Severity drives styling in the host (`NotificationSeverity`).

## Related

- [Shell](shell.md)
- [Dialogs](dialogs.md)
- [Globalization](globalization.md) — localized notification text via `ITranslationService`
