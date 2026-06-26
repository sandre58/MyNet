<div align="center">

# MyNet.UI

<img src="../../assets/MyNetUI.png" alt="MyNet.UI" width="96" height="96" />

*UI-framework-agnostic presentation layer: view models, shell, dialogs, navigation, notifications, and toasts. You provide concrete UI (WPF, Avalonia, etc.).*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.UI)](https://www.nuget.org/packages/MyNet.UI)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Navigation** | View locators and navigation services |
| **Dialogs** | Presenter abstraction and shell host patterns |
| **Notifications** | Toast and notification managers for MVVM |
| **Theming** | Contracts implemented by your UI host (WPF, Avalonia, etc.) |

---

## Installation

```bash
dotnet add package MyNet.UI
```

## Quick start (DI)

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI;
using MyNet.UI.Dialogs;
using MyNet.UI.Locators;

var services = new ServiceCollection();
services.AddUi(b => b
    .ConfigureViewLocators(r => r.Register(typeof(MainViewModel), typeof(MainView)))
    .ConfigureDialogs(d => d.AddPresenter<MyDialogPresenter>()));

var provider = services.BuildServiceProvider();
provider.UseUi();
```

Or register modules individually (`AddViewLocators`, `AddDialogs`, `AddNavigation`, …) — see the [UI guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/ui.md).

- **Dialogs:** implement `IDialogPresenter` — see [Dialogs guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/dialogs.md)
- **Notifications / toasts:** bind UI to `INotificationsManager.Notifications` and `IToastManager.Toasts` — see [Notifications & toasts guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/notifications-and-toasts.md#client-implementation-toast-overlay-control) for the toast overlay control
- **Shell:** register `ShellHostViewModel` in your host
- **Navigation:** implement `IViewHost` + middleware in your host — see [Navigation guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/navigation.md)
- **Theming:** register `IThemeService` + `IThemeBaseRegistry` in the host; `UseUi()` configures `ThemeManager` when both are present — see [Theming guide](https://github.com/sandre58/MyNet2/blob/main/docs/guides/theming.md)




---
## Documentation

- [UI presentation layer](https://github.com/sandre58/MyNet2/blob/main/docs/guides/ui.md) — locators
- [Navigation](https://github.com/sandre58/MyNet2/blob/main/docs/guides/navigation.md) — journal, guards, client view host
- [Dialogs](https://github.com/sandre58/MyNet2/blob/main/docs/guides/dialogs.md)
- [Notifications & toasts](https://github.com/sandre58/MyNet2/blob/main/docs/guides/notifications-and-toasts.md)
- [Shell](https://github.com/sandre58/MyNet2/blob/main/docs/guides/shell.md)
- [Theming](https://github.com/sandre58/MyNet2/blob/main/docs/guides/theming.md)
- [Documentation index](https://github.com/sandre58/MyNet2/blob/main/docs/index.md)




---
## Related packages

- [MyNet.Observable](https://www.nuget.org/packages/MyNet.Observable)
- [MyNet.Globalization](https://www.nuget.org/packages/MyNet.Globalization)
- [MyNet.IO](https://www.nuget.org/packages/MyNet.IO)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
