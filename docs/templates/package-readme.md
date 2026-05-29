# Package README template

Use this template for every packable project under `src/`. Keep it short (NuGet gallery + repo root). Full guides live under `docs/guides/`.

Replace placeholders: `{PackageId}`, `{Title}`, `{OneLiner}`, `{GuideSlug}`, `{GuideTitle}`.

---

```markdown
# {PackageId}

{OneLiner}

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/{PackageId})](https://www.nuget.org/packages/{PackageId})

**Target framework:** .NET 10

## Installation

```bash
dotnet add package {PackageId}
```

## Quick start

<!-- Minimal, compilable example. No host framework assumptions unless required. -->

## Related packages

<!-- Optional: sibling MyNet packages consumers often need -->

## Documentation

- [{GuideTitle}](https://github.com/sandre58/MyNet/blob/main/docs/guides/{GuideSlug}.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
- [Getting started](https://github.com/sandre58/MyNet/blob/main/docs/getting-started.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
```
