# MyNet

Modular **.NET 10** libraries for MVVM desktop applications: observable models, UI shell, globalization, geography, mail, HTTP, and shared primitives.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)

## Documentation

| Start here | Description |
|------------|-------------|
| [**Documentation index**](docs/index.md) | All packages, guides, maturity matrix |
| [**Getting started**](docs/getting-started.md) | Typical package sets and DI setup |
| [**System guides**](docs/guides/README.md) | 14 topic guides (Observable, UI, Dialogs, Shell, …) |
| [**Documentation backlog**](docs/TODO.md) | Gaps and priorities |
| [**Package README template**](docs/templates/package-readme.md) | Template for `src/*/README.md` |

## Repository layout

```
src/           Packable libraries (each with README.md for NuGet)
tests/         Unit tests (*.Tests → not packed)
docs/          Guides and reference documentation (English)
build/         MSBuild props (package, coverage, analyzers)
```

## Build & pack

```bash
dotnet build MyNet.slnx
dotnet pack MyNet.slnx -c Release
```

Packages output: `packages/` (see `build/package.props`).

## License

MIT — see [LICENSE](LICENSE).
