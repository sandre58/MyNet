# Theming (`IThemeService`)

**Package:** [MyNet.UI](../../src/MyNet.UI/README.md) · namespace `MyNet.UI.Theming`

Application-wide **theme state**: base appearance (light / dark / high contrast), primary and accent brand colors, optional foreground overrides, and hooks for pushing resources into your UI framework.

`MyNet.UI` defines **contracts and view-model integration** only. Your WPF, Avalonia, or MAUI host registers concrete `IThemeService` and `IThemeBaseRegistry` implementations that apply colors and resource dictionaries to real controls.

## Architecture

```text
┌─────────────────────────────────────────────────────────┐
│  Host UI (WPF / Avalonia)                                │
│  ResourceDictionary / Styles — listens to ThemeChanged   │
└───────────────────────────┬─────────────────────────────┘
                            │ implements
┌───────────────────────────▼─────────────────────────────┐
│  IThemeService          — state + Apply* + ThemeChanged   │
│  IThemeBaseRegistry     — Light / Dark / HighContrast     │
│  IThemeExtension        — extra resource keys per theme   │
└───────────────────────────┬─────────────────────────────┘
                            │ consumed by
┌───────────────────────────▼─────────────────────────────┐
│  DisplayPreferencesViewModel  — full color preferences  │
│  ShellThemeViewModel            — quick light/dark toggle │
│  ThemeManager (static)          — legacy / XAML code-behind│
└─────────────────────────────────────────────────────────┘
```

| Type | Role |
|------|------|
| `Theme` | Immutable snapshot: base + primary/accent colors + optional foregrounds |
| `IThemeBase` | Named base (`Light`, `Dark`, `HighContrast`) with `IsDark` / `IsHighContrast` |
| `IThemeService` | Apply changes, expose `CurrentTheme`, raise `ThemeChanged` |
| `IThemeBaseRegistry` | Catalog of bases for preferences UI |
| `IThemeExtension` | Supply `IDictionary<string, object?>` resources when theme changes |
| `ThemeManager` | Static facade over DI services (optional) |

There is **no** `AddTheming()` in `MyNet.UI` — register implementations in the **host project**.

---

## Registration

### 1. Implement services in the host

```csharp
// YourApp.UI/Theming/ThemeService.cs
using MyNet.UI.Theming;

public sealed class ThemeService : IThemeService
{
    private readonly List<IThemeExtension> _baseExtensions = [];
    private readonly List<IThemeExtension> _primaryExtensions = [];
    private readonly List<IThemeExtension> _accentExtensions = [];

    public Theme CurrentTheme { get; private set; }

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public ThemeService(IThemeBaseRegistry registry)
    {
        CurrentTheme = new Theme(registry.Light, "#2563EB", "#F59E0B");
    }

    public void ApplyTheme(Theme theme)
    {
        CurrentTheme = theme;
        NotifyChanged();
    }

    public void ApplyBaseTheme(IThemeBase baseTheme) =>
        ApplyTheme(CurrentTheme with { Base = baseTheme });

    public void ApplyPrimary(string color, string? foreground = null) =>
        ApplyTheme(CurrentTheme with
        {
            PrimaryColor = color,
            PrimaryForegroundColor = foreground
        });

    public void ApplyAccent(string color, string? foreground = null) =>
        ApplyTheme(CurrentTheme with
        {
            AccentColor = color,
            AccentForegroundColor = foreground
        });

    public void UpdateTheme(Func<Theme, Theme> update) =>
        ApplyTheme(update(CurrentTheme));

    public IThemeService AddBaseExtension(IThemeExtension extension)
    {
        _baseExtensions.Add(extension);
        return this;
    }

    public IThemeService AddPrimaryExtension(IThemeExtension extension)
    {
        _primaryExtensions.Add(extension);
        return this;
    }

    public IThemeService AddAccentExtension(IThemeExtension extension)
    {
        _accentExtensions.Add(extension);
        return this;
    }

    private void NotifyChanged()
    {
        // 1) Merge resources from extensions (see below)
        // 2) Push brushes into Application.Current.Resources (WPF) or equivalent
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(CurrentTheme));
    }
}
```

```csharp
// YourApp.UI/Theming/ThemeBaseRegistry.cs
public sealed class ThemeBaseRegistry : IThemeBaseRegistry
{
    private readonly Dictionary<string, IThemeBase> _byName = new(StringComparer.OrdinalIgnoreCase);

    public ThemeBaseRegistry()
    {
        Register(Light = new ThemeBase("Light", isDark: false, isHighContrast: false));
        Register(Dark = new ThemeBase("Dark", isDark: true, isHighContrast: false));
        Register(HighContrast = new ThemeBase("HighContrast", isDark: true, isHighContrast: true));
    }

    public IThemeBase Light { get; }
    public IThemeBase Dark { get; }
    public IThemeBase HighContrast { get; }

    public IReadOnlyCollection<IThemeBase> AvailableBases => _byName.Values.ToList();

    public void Register(IThemeBase themeBase) => _byName[themeBase.Name] = themeBase;

    public IThemeBase? Get(string name) =>
        _byName.TryGetValue(name, out var b) ? b : null;

    private sealed class ThemeBase(string name, bool isDark, bool isHighContrast) : IThemeBase
    {
        public string Name { get; } = name;
        public bool IsDark { get; } = isDark;
        public bool IsHighContrast { get; } = isHighContrast;
    }
}
```

### 2. Register in DI

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Theming;
using YourApp.UI.Theming;

var services = new ServiceCollection();

services.AddSingleton<IThemeBaseRegistry, ThemeBaseRegistry>();
services.AddSingleton<IThemeService, ThemeService>();

services.AddShell();
services.AddShellPreferences(); // DisplayPreferencesViewModel

var provider = services.BuildServiceProvider();
provider.UseThemeManager(); // wires ThemeManager static bridge
// or: provider.UseUi(); // configures ThemeManager only when both services are registered
```

`UseThemeManager()` calls `ThemeManager.Configure(themeService, registry)` — required if legacy code or XAML uses `ThemeManager` static members.

`UseThemeManagerIfAvailable()` (also invoked from `UseUi()`) configures the static bridge only when both `IThemeService` and `IThemeBaseRegistry` are registered; otherwise it is a no-op (useful for tests and hosts without theming).

### 3. Subscribe in the UI layer

On `ThemeChanged`, update application resources:

```csharp
// WPF example — App.xaml.cs or dedicated ThemeApplier
themeService.ThemeChanged += (_, e) =>
{
    var t = e.CurrentTheme;
    var app = Application.Current;
    app.Resources["PrimaryBrush"] = BrushFromHex(t.PrimaryColor);
    app.Resources["AccentBrush"] = BrushFromHex(t.AccentColor);
    // Swap merged dictionary for t.Base.Name (Light/Dark)
};
```

Keep **all framework-specific code** in the host; `MyNet.UI` view models only call `IThemeService` methods.

---

## `Theme` model

```csharp
public sealed record Theme(
    IThemeBase Base,
    string PrimaryColor,
    string AccentColor,
    string? PrimaryForegroundColor = null,
    string? AccentForegroundColor = null);
```

Colors are **strings** (typically `#RRGGBB` or `#AARRGGBB`). The host parses them for the target UI stack.

When `PrimaryForegroundColor` / `AccentForegroundColor` are `null`, the host may compute contrast automatically (see `DisplayPreferencesViewModel.AutoPrimaryForegroundColor`).

---

## `IThemeService` API

| Member | Behavior |
|--------|----------|
| `CurrentTheme` | Last applied theme |
| `ApplyTheme(Theme)` | Replace entire theme |
| `ApplyBaseTheme(IThemeBase)` | Change base only (light ↔ dark) |
| `ApplyPrimary(color, foreground?)` | Update primary brand color |
| `ApplyAccent(color, foreground?)` | Update accent color |
| `UpdateTheme(Func<Theme, Theme>)` | Functional update (immutable `Theme` record) |
| `AddBaseExtension` / `AddPrimaryExtension` / `AddAccentExtension` | Register resource contributors (fluent) |
| `ThemeChanged` | Fired after each apply |

### Functional update

```csharp
_themeService.UpdateTheme(t => t with
{
    PrimaryColor = "#10B981",
    AccentColor = "#6366F1"
});
```

---

## `IThemeExtension`

Extensions merge extra keyed resources when the theme changes (platform brushes, thicknesses, etc.):

```csharp
public interface IThemeExtension
{
    IDictionary<string, object?> GetResources(Theme theme);
}
```

Example — WPF brush provider:

```csharp
public sealed class WpfBrushThemeExtension : IThemeExtension
{
    public IDictionary<string, object?> GetResources(Theme theme) => new Dictionary<string, object?>
    {
        ["PrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.PrimaryColor)!),
        ["AccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.AccentColor)!),
    };
}

// During service setup:
services.AddSingleton<IThemeService>(sp =>
{
    var registry = sp.GetRequiredService<IThemeBaseRegistry>();
    return new ThemeService(registry)
        .AddPrimaryExtension(new WpfBrushThemeExtension())
        .AddAccentExtension(new WpfBrushThemeExtension());
});
```

Chain extensions on the concrete `ThemeService` in your host (interface returns `IThemeService` for chaining).

---

## `IThemeBaseRegistry`

| Member | Purpose |
|--------|---------|
| `Light`, `Dark`, `HighContrast` | Well-known defaults |
| `AvailableBases` | All bases for preferences combo box |
| `Register(IThemeBase)` | Add custom variant (e.g. "OLED Black") |
| `Get(name)` | Lookup by name |

`DisplayPreferencesViewModel` populates `AvailableBases` from the registry on construction.

---

## `ThemeManager` (static bridge)

For code that cannot use DI (code-behind, legacy helpers):

```csharp
using MyNet.UI.Theming;

// After UseThemeManager():
ThemeManager.ApplyBase(ThemeManager.Dark);
ThemeManager.ApplyPrimaryColor("#2563EB");
ThemeManager.ApplyAccentColor("#F59E0B", foreground: "#FFFFFF");

ThemeManager.ThemeChanged += (_, e) => { /* sync UI */ };
```

| Static member | Delegates to |
|---------------|--------------|
| `Light`, `Dark`, `HighContrast` | Registry |
| `AvailableBases`, `GetBase`, `Register` | Registry |
| `ApplyBase`, `ApplyTheme`, `ApplyPrimaryColor`, `ApplyAccentColor` | `IThemeService` |
| `CurrentTheme` | `IThemeService.CurrentTheme` |

Handlers attached to `ThemeManager.ThemeChanged` **before** `Configure` are queued and attached when `Configure` runs (see tests).

---

## Built-in view models

### Shell quick toggle — `ShellThemeViewModel`

Registered by `AddShell()`. Exposes `IsDark`, `IsDarkCommand`, `IsLightCommand`.

```csharp
// Switching IsDark calls:
_themeService.ApplyBaseTheme(isDark ? _themeBaseRegistry.Dark : _themeBaseRegistry.Light);
```

Bind toolbar toggle to `IsDark` in the shell chrome view.

### Preferences page — `DisplayPreferencesViewModel`

Registered by `AddShellPreferences()`. Properties:

| Property | Calls |
|----------|--------|
| `ThemeBase` | `ApplyBaseTheme` |
| `PrimaryColor` | `ApplyPrimary` |
| `AccentColor` | `ApplyAccent` |
| `PrimaryForegroundColor` / `AccentForegroundColor` | `ApplyPrimary` / `ApplyAccent` with manual foreground |
| `AutoPrimaryForegroundColor` / `AutoAccentForegroundColor` | Pass `null` foreground for auto contrast |

Subscribes to `ThemeChanged` and syncs UI without feedback loops (`_syncingFromTheme` flag).

Add the page to your `PreferencesViewModel` page list in the host.

---

## Startup sequence (checklist)

1. Register `IThemeBaseRegistry` and `IThemeService` in DI.
2. Optionally chain `IThemeExtension` instances on the service.
3. `services.AddShell()` + `AddShellPreferences()`.
4. Build provider → `provider.UseThemeManager()`.
5. Apply initial theme: `themeService.ApplyTheme(...)` or load from user settings.
6. Host subscribes to `ThemeChanged` and updates resource dictionaries.
7. Register `ShellHostViewModel` and bind shell chrome + preferences views.

---

## Persistence

`MyNet.UI` does not persist theme settings. Typical pattern:

```csharp
// On ThemeChanged — save to IRegistryService / settings file
_themeService.ThemeChanged += (_, e) =>
{
    settings.SaveTheme(e.CurrentTheme);
};

// On startup — restore
var saved = settings.LoadTheme();
if (saved is not null)
    themeService.ApplyTheme(saved);
```

Use [IO & platform](io-platform.md) registry abstractions or app-specific storage.

---

## Testing

`MyNet.UI.Tests` uses fakes — see `tests/MyNet.UI.Tests/Theming/ThemeManagerTests.cs`:

- `FakeThemeService` / `FakeThemeBaseRegistry` mirror `IThemeService` behavior with immutable `Theme` updates.
- `ThemeManager.ResetForTesting()` clears static state between tests.

Mock `IThemeService` for view-model tests (`DisplayPreferencesViewModelTests`, `ShellThemeViewModelTests`).

---

## Common mistakes

| Issue | Fix |
|-------|-----|
| `ThemeManager.ApplyBase` throws | Call `UseThemeManager()` after building DI |
| Preferences do not update UI | Host must handle `ThemeChanged` and merge resources |
| Light/dark toggle does nothing | Implement `ApplyBaseTheme` to swap base resource dictionary |
| Two sources of truth | Single `IThemeService` singleton; view models only call service |
| Colors as `SolidColorBrush` in `Theme` | Keep `Theme` as strings; convert in host / extensions |

---

## Related

- [Shell](shell.md) — `ShellThemeViewModel`, preferences workspace
- [UI presentation layer](ui.md)
- [Getting started](../getting-started.md)

## Source files

| File | Content |
|------|---------|
| `src/MyNet.UI/Theming/IThemeService.cs` | Service contract |
| `src/MyNet.UI/Theming/IThemeBaseRegistry.cs` | Registry contract |
| `src/MyNet.UI/Theming/Theme.cs` | Theme record |
| `src/MyNet.UI/Theming/ThemeManager.cs` | Static bridge |
| `src/MyNet.UI/Theming/Extensions/ServiceCollectionExtensions.cs` | `UseThemeManager()`, `UseThemeManagerIfAvailable()` |
| `src/MyNet.UI/Extensions/ServiceCollectionExtensions.cs` | `AddUi()`, `UseUi()` |
| `src/MyNet.UI/Extensions/UiBuilder.cs` | Optional UI DI configuration |
| `src/MyNet.UI/ViewModels/Preferences/DisplayPreferencesViewModel.cs` | Preferences UI |
| `src/MyNet.UI/ViewModels/Shell/Chrome/ShellThemeViewModel.cs` | Shell toggle |
