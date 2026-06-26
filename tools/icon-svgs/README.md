# Package icon SVG sources

Stroke icons (24×24 viewBox, white `#FFFFFF` strokes) rasterized to `assets/*.png` by `tools/PackageIconGenerator`.

Paths are inspired by [Tabler Icons](https://tabler.io/icons) (MIT). You can replace any file with an official Tabler export (outline, 24px) — keep `stroke="#FFFFFF"` or adjust the generator to tint strokes.

| SVG | Package | Meaning |
|-----|---------|---------|
| `metadata.svg` | MyNet.Metadata | Braces `{ }` — schema / metadata |
| *(glyph)* `HTTP` | MyNet.Http | Bold **HTTP** label (Skia text, not SVG) |
| `observable.svg` | MyNet.Observable | Activity pulse — reactive / observable |
| `typography.svg` | MyNet.Text | Document with text lines |
| `utilities.svg` | MyNet.Utilities | Puzzle — helpers / building blocks |
| `avalonia.svg` | *(unused)* | Former « A » mark |
| `xaml-binding.svg` | MyNet.Avalonia | Braces + binding dot |
| `controls-panel.svg` | MyNet.Avalonia.Controls | Sliders / control panel |
| `dialog.svg` | MyNet.Avalonia.Extended | Dialog / message bubble |
| `palette.svg` | MyNet.Avalonia.Theme | Theming / styling |
| `theme-control.svg` | MyNet.Avalonia.Theme.Controls | Palette + toggle control |

Full registry (all repos): [`icon-registry.md`](../icon-registry.md).

## Add or change an icon

1. Add or edit `*.svg` in this folder.
2. Update `tools/package-icons.json` (`svg`, `base`, `accent`, `label`).
3. Run from repo root:

```powershell
pwsh -File tools/generate-package-icons.ps1
```

## Layout (fixed)

- Pictogram in the upper area (no overlap with footer).
- Footer band (24px) with package code (`PR`, `ML`, …).
- Thin divider line between pictogram and footer (not under the label).
