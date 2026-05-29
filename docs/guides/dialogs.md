# Dialogs

**Package:** [MyNet.UI](../../src/MyNet.UI/README.md)

Content dialogs, message boxes, and file dialogs with a **UI-framework-agnostic** core. Your host registers a platform **`IDialogPresenter`** to show real UI; without a presenter, **headless** strategies return safe defaults (useful in tests).

## Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Dialogs;

var services = new ServiceCollection();

services.AddDialogs(builder =>
{
    builder.AddPresenter<MyWpfDialogPresenter>(); // implements IDialogPresenter
});

// Optional: custom message box or file dialog implementations
// services.AddMessageBoxFactory<MyMessageBoxFactory>();
// services.AddFileDialogService<MyFileDialogService>();
```

`AddDialogs` registers:

| Service | Default implementation |
|---------|------------------------|
| `IContentDialogService` | `ContentDialogService` |
| `IDialogService` | Facade: content + message box + file |
| `IMessageBoxService` | `MessageBoxService` |
| `IFileDialogService` | `CancelledFileDialogService` (no UI until replaced) |
| `IDialogStrategy` | `HeadlessDialogStrategy`, `PresenterDialogStrategy` |

Call **`AddShell()`** after `AddDialogs()` so shell drawer coordination can subscribe to dialog events.

## Architecture

```text
Application code
    → IContentDialogService / IMessageBoxService / IDialogService
        → IDialogStrategy (highest priority that CanPresent)
            → HeadlessDialogStrategy (tests, server)
            → PresenterDialogStrategy → IDialogPresenter (WPF / Avalonia / …)
```

| Type | Role |
|------|------|
| `IDialog` / `IDialog<TResult>` | Dialog view model |
| `DialogOptions` | Presentation options (owner, style, etc.) |
| `DialogResult` / `DialogResult<T>` | Outcome + optional typed result |
| `IDialogPresenter` | Platform UI: `PresentAsync`, `CloseAsync`, `Priority` |
| `IContentDialogService` | `ShowAsync`, `Create` builder, `OpenedDialogs` |

## Content dialogs

```csharp
using MyNet.UI.Dialogs.ContentDialogs;

public sealed class ConfirmDialogViewModel : ObservableObject, IDialog<bool>
{
    public bool Result { get; private set; }

    public void Confirm() => Close(DialogResult.Ok(true));
    public void Cancel() => Close(DialogResult.Cancel<bool>());
}

// In a view model:
public class EditorViewModel(IContentDialogService dialogs)
{
    public async Task<bool> AskSaveAsync()
    {
        var dialog = new ConfirmDialogViewModel();
        var result = await dialogs.ShowAsync(dialog, cancellationToken: ct);
        return result is { IsOk: true, Value: true };
    }
}
```

Fluent builder:

```csharp
var result = await dialogs
    .Create(myDialog)
    .WithTitle("Unsaved changes")
    .ShowAsync(cancellationToken);
```

Lifecycle events: `DialogOpened`, `DialogClosed` on `IContentDialogService`.

## Message boxes

```csharp
using MyNet.UI.Dialogs;

public class MyViewModel(IMessageBoxService messageBox)
{
    public async Task ConfirmDeleteAsync()
    {
        var result = await messageBox.ShowAsync(
            "Delete this item?",
            title: "Confirm",
            buttons: MessageBoxResultOption.YesNo,
            icon: MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
            await DeleteAsync();
    }
}
```

With a registered presenter, results come from the UI. Headless mode returns defaults (e.g. OK for simple dialogs) — see `tests/MyNet.UI.Tests/Dialogs/MessageBoxServiceTests.cs`.

## File dialogs

Replace the default cancelled implementation:

```csharp
services.AddDialogs();
services.AddFileDialogService<MyAvaloniaFileDialogService>();
```

Use via `IDialogService` or `IFileDialogService` depending on your app layer.

## Implementing `IDialogPresenter`

```csharp
using MyNet.UI.Dialogs.ContentDialogs;

public sealed class MyDialogPresenter : IDialogPresenter
{
    public int Priority => 100;

    public bool CanPresent(IDialog dialog, DialogOptions? options) => true;

    public async Task<DialogResult<bool>> PresentAsync(
        IDialog dialog,
        DialogOptions options,
        CancellationToken cancellationToken)
    {
        // Show modal bound to dialog view model; return DialogResult when closed
        throw new NotImplementedException();
    }

    public Task CloseAsync(IDialog dialog) => Task.CompletedTask;
}
```

Higher `Priority` wins when multiple presenters are registered.

## Testing

```csharp
services.AddDialogs();
// Headless only — no presenter
var dialogService = provider.GetRequiredService<IContentDialogService>();

services.AddDialogs(b => b.AddPresenter<StubMessageBoxPresenter>());
```

See `tests/MyNet.UI.Tests/Dialogs/`.

## Related

- [UI presentation layer](ui.md)
- [Shell](shell.md)
- [Observable models](observable.md) — dialog view models
