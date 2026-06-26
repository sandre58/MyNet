# Package icon registry

Single source of truth for NuGet package icons across My .NET repositories.

**Visual encoding:** solution color (gradient) × category pictogram (SVG) × package label (2-letter footer).

Generator: [`MyNet.Tools.PackageIconGenerator`](MyNet.Tools.PackageIconGenerator/) — run from any repo via `--manifest`, `--svg-dir`, `--output`.

## Solution palettes

| Ecosystem | Base | Accent range | Repos |
|-----------|------|--------------|-------|
| MyNet core | `#1E1B4B` | indigo / cyan / emerald / amber accents *(no violet — reserved for Avalonia)* | `MyNet` |
| MyAvalonia | `#581C87` → accent nuance | violet / fuchsia (shared base, per-package accent) | `MyAvalonia` |
| MyWpf | `#1E3A8A` | steel blue *(planned)* | `MyWpf` |
| MyXaml | `#064E3B` | emerald *(planned)* | `MyXaml` |

**Rule:** never reuse the exact same `base` + `accent` + `svg` + `label` combination across packages.

**Extension rule:** a satellite package (e.g. `MyNet.Avalonia.Geography`) may reuse the **pictogram** of its MyNet counterpart, but must use its **solution palette** and a **distinct label**.

## MyNet

| Label | PackageId | PNG | SVG | Base | Accent | Shared with |
|-------|-----------|-----|-----|------|--------|-------------|
| PR | MyNet.Primitives | MyNetPrimitives.png | box.svg | #1E1B4B | #6366F1 | — |
| TX | MyNet.Text | MyNetText.png | typography.svg | #1E1B4B | #6366F1 | — |
| UT | MyNet.Utilities | MyNetUtilities.png | utilities.svg | #1E1B4B | #818CF8 | — |
| GN | MyNet.Generator | MyNetGenerator.png | bolt.svg | #1E1B4B | #818CF8 | — |
| RF | MyNet.Reflection | MyNetReflection.png | flip.svg | #1E1B4B | #818CF8 | — |
| MD | MyNet.Metadata | MyNetMetadata.png | metadata.svg | #1E1B4B | #818CF8 | — |
| TP | MyNet.Temporal | MyNetTemporal.png | clock.svg | #1E1B4B | #818CF8 | — |
| CL | MyNet.Collections | MyNetCollections.png | stack.svg | #1E1B4B | #4F46E5 | — |
| MS | MyNet.Messaging | MyNetMessaging.png | message.svg | #1E1B4B | #4F46E5 | — |
| OB | MyNet.Observable | MyNetObservable.png | observable.svg | #1E1B4B | #06B6D4 | — |
| UI | MyNet.UI | MyNetUI.png | window.svg | #1E1B4B | #06B6D4 | — |
| GL | MyNet.Globalization | MyNetGlobalization.png | world.svg | #1E1B4B | #34D399 | — |
| HU | MyNet.Humanizer | MyNetHumanizer.png | user.svg | #1E1B4B | #34D399 | — |
| GE | MyNet.Geography | MyNetGeography.png | map-pin.svg | #1E1B4B | #F59E0B | Geography.* |
| GO | MyNet.Google | MyNetGoogle.png | search.svg | #1E1B4B | #F59E0B | — |
| IO | MyNet.IO | MyNetIO.png | folder.svg | #1E1B4B | #94A3B8 | — |
| PW | MyNet.Platform.Windows | MyNetPlatformWindows.png | layout-grid.svg | #1E1B4B | #38BDF8 | — |
| HT | MyNet.Http | MyNetHttp.png | *(glyph HTTP)* | #1E1B4B | #F87171 | — |
| ML | MyNet.Mail | MyNetMail.png | mail.svg | #1E1B4B | #FB7185 | Mail.MailKit |
| FK | MyNet.Fakers | MyNetFakers.png | dice.svg | #1E1B4B | #F472B6 | — |

Manifest: [`package-icons.json`](package-icons.json)

## MyAvalonia

| Label | PackageId | PNG | SVG | Base | Accent | Notes |
|-------|-----------|-----|-----|------|--------|-------|
| AV | MyNet.Avalonia | MyAvalonia.png | xaml-binding.svg | #581C87 | #C026D3 | XAML markup / `{Binding}` |
| AC | MyNet.Avalonia.Controls | MyAvaloniaControls.png | controls-panel.svg | #581C87 | #D946EF | Sliders / knobs |
| AE | MyNet.Avalonia.Extended | MyAvaloniaExtended.png | dialog.svg | #581C87 | #A855F7 | Dialogs |
| AG | MyNet.Avalonia.Geography | MyAvaloniaGeography.png | map-pin.svg | #581C87 | #E879F9 | Geography extension |
| AT | MyNet.Avalonia.Theme | MyAvaloniaTheme.png | palette.svg | #581C87 | #9333EA | Theming |
| TC | MyNet.Avalonia.Theme.Controls | MyAvaloniaThemeControls.png | theme-control.svg | #581C87 | #C084FC | Palette + control toggle |

Manifest: `MyAvalonia/tools/package-icons.json` (generated via shared tool + MyNet SVG library).

## MyWpf *(planned)*

| Label | PackageId | PNG | SVG |
|-------|-----------|-----|-----|
| WP | MyNet.Wpf | MyWpf.png | window.svg |
| PP | MyNet.Wpf.Presentation | MyWpfPresentation.png | layout-grid.svg |
| … | … | … | … |

## MyXaml *(planned)*

| Label | PackageId | PNG | SVG |
|-------|-----------|-----|-----|
| XM | MyNet.Xaml.Merger | MyXamlMerger.png | tool.svg |
| XB | MyNet.Xaml.Merger.MSBuild | MyXamlMergerMSBuild.png | bolt.svg |

## Add a new package

1. Pick **solution palette** (table above).
2. Pick or create an **SVG** in [`icon-svgs/`](icon-svgs/) (Tabler-style, white strokes).
3. Assign a unique **2-letter label** in this registry.
4. Add an entry to the repo's `tools/package-icons.json`.
5. Regenerate PNGs (see below).
6. Update this file.

## Regenerate

**MyNet (local defaults):**

```powershell
dotnet run --project tools/MyNet.Tools.PackageIconGenerator -c Release
```

**MyAvalonia (shared generator + MyNet SVGs):**

```powershell
powershell -File tools/generate-package-icons.ps1
```

**Any repo (explicit paths):**

```powershell
dotnet run --project ../MyNet/tools/MyNet.Tools.PackageIconGenerator -c Release -- `
  --manifest tools/package-icons.json `
  --svg-dir ../MyNet/tools/icon-svgs `
  --output assets
```
