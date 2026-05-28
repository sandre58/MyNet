<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <img src="../../assets/MyNetUI.png" width="128" alt="MyNetUI">
</div>

<h1 align="center">My .NET UI</h1>

[![MIT License](https://img.shields.io/github/license/sandre58/mynet?style=for-the-badge)](https://github.com/sandre58/mynet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.UI?style=for-the-badge)](https://www.nuget.org/packages/MyNet.UI)

A comprehensive library for simplifying common GUI functionalities in .NET applications.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](#)
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-purple)](#)
[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-purple)](#)
[![C#](https://img.shields.io/badge/language-C%23-blue)](#)

---

## Installation

Install via NuGet:

```bash
dotnet add package MyNet.UI
```

## Features

- **Notification System**: Real-time, customizable notifications.
- **MessageBox Integration**: Standard and customizable dialog boxes.
- **Navigation Framework**: Manage navigation and history between views/pages.
- **Toaster Notifications**: Non-intrusive, interactive toast notifications.
- **Theme Management**: Dynamic theming, light/dark mode, and theme persistence.

## Example Usage

```csharp
using MyNet.UI.Notifications;

// Show a notification
NotificationService.Show("Hello, user!");

// Show a MessageBox
MessageBoxService.Show("Are you sure?", "Confirmation");

// Theming (register implementations in your WPF/Avalonia host, then):
services.AddSingleton<IThemeBaseRegistry, ThemeVariantsRegistry>();
services.AddSingleton<IThemeService, ThemeService>();
// After BuildServiceProvider():
serviceProvider.UseThemeManager();

themeService.ApplyBaseTheme(themeBaseRegistry.Dark);
```

## License

Copyright © Stéphane ANDRE.

Distributed under the MIT License. See [LICENSE](../../LICENSE) for details.