# MyNet.UI

UI-framework-agnostic presentation layer: view models, shell, dialogs, navigation, notifications, and toasts. You provide concrete UI (WPF, Avalonia, etc.).

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet2)](https://github.com/sandre58/MyNet2/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.UI)](https://www.nuget.org/packages/MyNet.UI)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.UI
```

## Quick start (DI)

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Dialogs;
using MyNet.UI.Locators;
using MyNet.UI.Navigation;
using MyNet.UI.Notifications;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels;

var services = new ServiceCollection();
services.AddViewLocators();
services.AddDialogs(b => b.AddPresenter<MyDialogPresenter>());
services.AddNavigation();
services.AddNotifications();
services.AddToasting();
services.AddShell();
```

- **Dialogs:** implement `IDialogPresenter` — see [Dialogs guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/dialogs.md)
- **Notifications / toasts:** bind UI to `INotificationsManager.Notifications` and `IToastManager.Toasts`
- **Shell:** register `ShellHostViewModel` in your host
- **Theming:** implement `IThemeService` + `IThemeBaseRegistry` in your host, then `UseThemeManager()` — see [Theming guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/theming.md)

## Documentation

- [UI presentation layer](https://github.com/sandre58/MyNet2/blob/main/docs/guides/ui.md) — locators, navigation
- [Dialogs](https://github.com/sandre58/MyNet2/blob/main/docs/guides/dialogs.md)
- [Notifications & toasts](https://github.com/sandre58/MyNet2/blob/main/docs/guides/notifications-and-toasts.md)
- [Shell](https://github.com/sandre58/MyNet2/blob/main/docs/guides/shell.md)
- [Theming](https://github.com/sandre58/MyNet2/blob/main/docs/guides/theming.md)
- [Documentation index](https://github.com/sandre58/MyNet2/blob/main/docs/index.md)

## Related packages

- [MyNet.Observable](https://www.nuget.org/packages/MyNet.Observable)
- [MyNet.Globalization](https://www.nuget.org/packages/MyNet.Globalization)
- [MyNet.IO](https://www.nuget.org/packages/MyNet.IO)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet2/blob/main/LICENSE).
