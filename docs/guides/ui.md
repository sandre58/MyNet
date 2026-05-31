# UI presentation layer

**Package:** [MyNet.UI](../../src/MyNet.UI/README.md)

UI-framework-agnostic **view models**, **shell**, **dialogs**, **navigation**, **notifications**, and **toasts**. You host concrete presenters in WPF, Avalonia, or another stack.

Code lives under [`src/MyNet.UI`](../../src/MyNet.UI).

## Architecture overview

```
┌─────────────────────────────────────────┐
│  Your UI framework (WPF / Avalonia)     │
│  Dialog presenter, Toast overlay,        │
│  Theme resources, Views                 │
└─────────────────┬───────────────────────┘
                  │ implements abstractions
┌─────────────────▼───────────────────────┐
│  MyNet.UI                               │
│  Shell, Navigation, Locators, Dialogs,  │
│  Notifications, Toasts                  │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│  MyNet.Observable, MyNet.Globalization  │
└─────────────────────────────────────────┘
```

### Shared modules

| Module | Folder | Role |
|--------|--------|------|
| Locators | [`Locators`](../../src/MyNet.UI/Locators) | ViewModel ↔ View resolution and MVVM instantiation |
| Navigation | [`Navigation`](../../src/MyNet.UI/Navigation) | App navigation (journal, guards, middleware) without WPF/Avalonia |
| Shell | ViewModels / Shell | Host, drawers, file menu |
| Dialogs | Dialogs | Content dialogs (host provides presenter) |
| Notifications / Toasts | Notifications, Toasting | Bind UI to manager collections |

### Dedicated guides

| Topic | Guide |
|-------|-------|
| App navigation (journal, guards, client host) | [Navigation](navigation.md) |
| Content dialogs, message boxes, file dialogs | [Dialogs](dialogs.md) |
| Notification center & toasts | [Notifications & toasts](notifications-and-toasts.md) |
| Shell, drawers, taskbar, preferences | [Shell](shell.md) |
| Theming (`IThemeService`, light/dark, colors) | [Theming](theming.md) |

---

## DI registration (typical)

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Dialogs;
using MyNet.UI.Locators;
using MyNet.UI.Navigation;
using MyNet.UI.Notifications;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels;

var services = new ServiceCollection();

services.AddViewLocators(r => r.Register(typeof(MainViewModel), typeof(MainView)));
services.AddDialogs(b => b.AddPresenter<MyDialogPresenter>()); // IDialogPresenter
services.AddNavigation().AddNavigationGuard<UnsavedChangesGuard>();
services.AddNotifications();
services.AddToasting();
services.AddShell();
services.AddShellPreferences();

// Host registers: ShellHostViewModel, views, theme services
```

See [Dialogs](dialogs.md), [Notifications & toasts](notifications-and-toasts.md), [Shell](shell.md), and [Theming](theming.md) for host responsibilities (`IDialogPresenter`, notification/toast collections, `IThemeService` + resource apply).

---

## Locators (View / ViewModel)

Resolves ViewModel ↔ View pairs and instantiates them in an MVVM architecture.

### DI registration

```csharp
services.AddViewLocators(configureResolver: resolver =>
{
    // Manual mappings (take priority over conventions)
    resolver.Register(typeof(SettingsViewModel), typeof(SettingsView));
});
```

Additional conventions (opt-in; register before or after `AddViewLocators` depending on desired priority):

```csharp
services
    .AddNamespaceConvention()           // ViewModels/Views segments, *View suffix only
    .AddParentNamespaceConvention()     // {Parent}.Views / {Parent}.ViewModels
    .AddAssemblyRootConvention("UI.Views") // {Assembly}.UI.Views.{Name}View
    .AddViewLocators();
```

By default, `AddViewLocators()` registers `SuffixConvention`, `ITypeResolver`, `IViewLocator`, `IViewModelLocator`, and `IViewFactory`.

### Components

| Service | Role |
|---------|------|
| `ITypeResolver` | Resolves the **view type** from a ViewModel (conventions + `Register`) |
| `IViewLocator` | Instantiates a **view**: DI first, otherwise `Activator` (parameterless constructor) |
| `IViewModelLocator` | Instantiates a **view model**: **strict DI** (must be registered) |
| `IViewFactory` | ViewModel → view instance (`Resolve` + `Get`) |

### Convention order

`TypeResolver` walks conventions in **DI registration order** and stops at the first match.

By default, only `SuffixConvention` is registered (swap `ViewModels`/`Views` + try suffixes: `View`, `Control`, `Page`, etc.).

Manual mappings via `resolver.Register(source, target)` **always override** conventions.

### Instantiation policy

- **Views**: register in DI for dependency injection; otherwise reflection instantiation.
- **ViewModels**: always registered in DI (`AddTransient`, `AddScoped`, etc.).

### Creating a view from a ViewModel

```csharp
public class MyShell(IViewFactory viewFactory)
{
    public void Show<TViewModel>()
        where TViewModel : class
    {
        var view = viewFactory.CreateView(typeof(TViewModel));
        // or: viewFactory.CreateView<TViewModel, MyView>();
    }
}
```

On failure (no mapping, incompatible type, instantiation error), `ViewResolutionException` is thrown.

### Supported project layouts

| Convention | ViewModel | View |
|------------|-----------|------|
| `SuffixConvention` (default) | `MyApp.ViewModels.PersonViewModel` | `MyApp.Views.PersonView` (or `PersonPage`, etc.) |
| `NamespaceConvention` | same | `MyApp.Views.PersonView` only |
| `ParentNamespaceConvention` | `MyApp.Features.ViewModels.XViewModel` | `MyApp.Features.Views.XView` |
| `AssemblyRootConvention` | `Any.Namespace.FooViewModel` | `{Assembly}.UI.Views.FooView` |

### Locator source files

- `Conventions/` — naming strategies (`TypeNamingConventionBase`, helpers)
- `Factories/ViewFactory.cs` — orchestration
- `ViewLocator.cs`, `ViewModelLocator.cs` — instantiation
- `ServiceCollectionExtensions.cs` — DI registration

See also [`src/MyNet.UI/Locators/README.md`](../../src/MyNet.UI/Locators/README.md) if present.

---

## Navigation

Journal-based app navigation (guards, middleware, back/forward) without tying to WPF or Avalonia. The host project wires views via middleware and locators — same idea as `IDialogPresenter` for dialogs.

```csharp
services.AddViewLocators().AddNavigation().AddNavigationMiddleware<ViewHostNavigationMiddleware>();
services.AddTransient<DashboardPage>();
```

Full API, pipeline, guards, and **client implementation** (`IViewHost`, view middleware, shell wiring): **[Navigation guide](navigation.md)**.

---

## Related packages

- [Observable models](observable.md)
- [Globalization](globalization.md)
- [IO & platform](io-platform.md)
- [Dialogs](dialogs.md) · [Notifications & toasts](notifications-and-toasts.md) · [Navigation](navigation.md) · [Shell](shell.md)

## Package README

[MyNet.UI README](../../src/MyNet.UI/README.md)
