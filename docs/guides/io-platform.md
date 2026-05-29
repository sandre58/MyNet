# IO & platform

**Packages:** [MyNet.IO](../../src/MyNet.IO/README.md), [MyNet.Platform.Windows](../../src/MyNet.Platform.Windows/README.md)

File system helpers, auto-save, portable registry abstractions, recent files — plus **Windows-specific** registry and MAPI adapters.

## MyNet.IO

No `AddIO()` extension in `MyNet.IO`; use types directly or register singletons in your host.

### File and directory helpers

```csharp
using MyNet.IO;

FileHelper.EnsureDirectoryExists(@"C:\Data\exports");
FileHelper.EnsureFileExists(path); // creates empty file if missing
FileHelper.TryDeleteFile(path);
```

`IDirectoryService` / `DirectoryService` — directory existence and item enumeration.

### Path and file extensions

`MyNet.IO.FileExtensions` — `FileExtension`, filters (`CommonFileFilters`), extension-based dialogs integration.

### Recent files

```csharp
using MyNet.IO.FileHistory;

// IRecentFilesService — track recently opened paths for shell file menu
```

Used with [Shell](shell.md) file menu in desktop apps.

### Auto-save

```csharp
using MyNet.IO.AutoSave;

var feature = new AutoSaveFeature();
feature.Enable();
feature.Changed += (_, e) => { /* IsEnabled changed */ };

// Subclass AutoSaveEngine for periodic save:
public sealed class DocumentAutoSaveEngine : AutoSaveEngine
{
    protected override Task SaveCoreAsync(CancellationToken cancellationToken)
    {
        // persist document
        return Task.CompletedTask;
    }
}
```

- `IAutoSaveFeature` — toggle auto-save on/off
- `IAutoSaveEngine` — `SetInterval`, `TriggerSaveAsync`, `Start` / `Stop`

See `tests/MyNet.IO.Tests/AutoSaveTests.cs`.

### Portable registry abstraction

`MyNet.IO.Registry` defines storage independent of OS:

| Type | Role |
|------|------|
| `IRegistryService` | `AddOrUpdate`, `Get`, `GetAll`, `Remove` |
| `IRegistryStore` | Physical store |
| `IRegistryNavigator` | Path navigation |
| `RegistryPath`, `RegistryEntry<T>` | Model |

Use for app settings trees without tying to Windows Registry API in core logic.

---

## MyNet.Platform.Windows

Target **Windows only** (`net10.0-windows` or similar in the host). Reference from executables, not from cross-platform libraries.

### Windows registry implementation

Binds `IRegistryService` to the real registry:

- `WindowsRegistryService`
- `WindowsRegistryStore`, `WindowsRegistryNavigator`

Tests: `tests/MyNet.Platform.Windows.Tests/WindowsRegistryTests.cs`.

```csharp
// Typical host registration (pattern — see Platform.Windows extensions in source)
services.AddSingleton<IRegistryService, WindowsRegistryService>();
```

### MAPI mail

Native Windows mail UI integration (when SMTP via MailKit is not desired). See [Mail guide](mail.md) for cross-platform SMTP.

### Authentication

Windows-specific authentication helpers under `MyNet.Platform.Windows` — use when integrating with OS identity.

---

## Choosing IO vs Platform.Windows

| Scenario | Package |
|----------|---------|
| Paths, auto-save, abstract settings | **MyNet.IO** |
| Windows registry backing store | **Platform.Windows** + IO registry interfaces |
| SMTP email | **MyNet.Mail.MailKit** |
| Native Windows mail compose | **Platform.Windows** MAPI |

---

## Related

- [Shell](shell.md) — recent files, workspace paths
- [Mail](mail.md)
- [Foundations](foundations.md)
