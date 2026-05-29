# NuGet package icons

128×128 PNG files packed via `build/package.props` when `assets/$(PackageIcon)` exists.

Icons are built from **SVG sources** in [`tools/icon-svgs`](../tools/icon-svgs/) (Tabler-style strokes, rasterized with SkiaSharp).

## Regenerate

```powershell
pwsh -File tools/generate-package-icons.ps1
```

Configuration: [`tools/package-icons.json`](../tools/package-icons.json).

## Shared icons

| File | Packages |
|------|----------|
| `MyNetGeography.png` | MyNet.Geography, MyNet.Geography.Resources, MyNet.Geography.Localization |
| `MyNetMail.png` | MyNet.Mail, MyNet.Mail.MailKit |

Each other packable project has its own `PackageIcon` file (see `tools/package-icons.json`).
