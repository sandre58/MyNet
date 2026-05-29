# MyNet documentation

MyNet is a modular **.NET 10** library suite published as NuGet packages. This index is the single entry point for package READMEs and system guides.

**Last reviewed:** 2026-05-29

## Documentation map

```text
docs/
├── index.md                      ← you are here
├── getting-started.md
├── TODO.md
├── templates/package-readme.md
└── guides/
    ├── README.md
    ├── foundations.md
    ├── collections-messaging.md
    ├── observable.md             ★ MVVM + metadata
    ├── ui.md                     ★ Locators + navigation
    ├── dialogs.md                ★ NEW
    ├── notifications-and-toasts.md ★ NEW
    ├── shell.md                  ★ NEW
    ├── theming.md                ★ IThemeService (host implements)
    ├── globalization.md          ★ Translation system
    ├── humanizer.md
    ├── geography.md
    ├── io-platform.md
    ├── http.md
    ├── mail.md
    └── fakers.md
```

## Start here

| Audience | Read first |
|----------|------------|
| New consumer | [Getting started](getting-started.md) → [Guides index](guides/README.md) |
| One NuGet package | [Package catalog](#package-catalog) → `src/<Package>/README.md` |
| Desktop MVVM app | [Getting started](getting-started.md) → [UI](guides/ui.md) + [Observable](guides/observable.md) |
| Contributor | [CONTRIBUTING.md](../CONTRIBUTING.md), [TODO](TODO.md), [.ai/architecture.md](../.ai/architecture.md) |

## Package catalog

### Foundations

| Package | Guide |
|---------|-------|
| [MyNet.Primitives](../src/MyNet.Primitives/README.md) | [Foundations](guides/foundations.md) |
| [MyNet.Text](../src/MyNet.Text/README.md) | [Foundations](guides/foundations.md) |
| [MyNet.Utilities](../src/MyNet.Utilities/README.md) | [Foundations](guides/foundations.md) |
| [MyNet.Generator](../src/MyNet.Generator/README.md) | [Foundations](guides/foundations.md) |
| [MyNet.Reflection](../src/MyNet.Reflection/README.md) | [Foundations](guides/foundations.md) |
| [MyNet.Metadata](../src/MyNet.Metadata/README.md) | [Observable → Metadata](guides/observable.md#metadata-generation) |
| [MyNet.Temporal](../src/MyNet.Temporal/README.md) | [Foundations](guides/foundations.md) |

### Data & messaging

| Package | Guide |
|---------|-------|
| [MyNet.Collections](../src/MyNet.Collections/README.md) | [Collections & messaging](guides/collections-messaging.md) |
| [MyNet.Messaging](../src/MyNet.Messaging/README.md) | [Collections & messaging](guides/collections-messaging.md) |

### MVVM & UI

| Package | Guide |
|---------|-------|
| [MyNet.Observable](../src/MyNet.Observable/README.md) | [Observable models](guides/observable.md) |
| [MyNet.UI](../src/MyNet.UI/README.md) | [UI](guides/ui.md) · [Dialogs](guides/dialogs.md) · [Notifications](guides/notifications-and-toasts.md) · [Shell](guides/shell.md) · [Theming](guides/theming.md) |

### Localization & geography

| Package | Guide |
|---------|-------|
| [MyNet.Globalization](../src/MyNet.Globalization/README.md) | [Globalization](guides/globalization.md) |
| [MyNet.Humanizer](../src/MyNet.Humanizer/README.md) | [Humanizer](guides/humanizer.md) |
| [MyNet.Geography](../src/MyNet.Geography/README.md) | [Geography](guides/geography.md) |
| [MyNet.Geography.Resources](../src/MyNet.Geography.Resources/README.md) | [Geography](guides/geography.md) |
| [MyNet.Geography.Localization](../src/MyNet.Geography.Localization/README.md) | [Geography](guides/geography.md) |
| [MyNet.Google](../src/MyNet.Google/README.md) | [Geography](guides/geography.md) |

### Infrastructure

| Package | Guide |
|---------|-------|
| [MyNet.IO](../src/MyNet.IO/README.md) | [IO & platform](guides/io-platform.md) |
| [MyNet.Platform.Windows](../src/MyNet.Platform.Windows/README.md) | [IO & platform](guides/io-platform.md) |
| [MyNet.Http](../src/MyNet.Http/README.md) | [HTTP clients](guides/http.md) |
| [MyNet.Mail](../src/MyNet.Mail/README.md) | [Mail](guides/mail.md) |
| [MyNet.Mail.MailKit](../src/MyNet.Mail.MailKit/README.md) | [Mail](guides/mail.md) |
| [MyNet.Fakers](../src/MyNet.Fakers/README.md) | [Fakers](guides/fakers.md) |

## Guide maturity

| Guide | Status |
|-------|--------|
| [observable.md](guides/observable.md) | Complete |
| [ui.md](guides/ui.md) | Complete (locators, navigation) |
| [dialogs.md](guides/dialogs.md) | Complete |
| [notifications-and-toasts.md](guides/notifications-and-toasts.md) | Complete |
| [shell.md](guides/shell.md) | Complete |
| [theming.md](guides/theming.md) | Complete |
| [globalization.md](guides/globalization.md) | Complete |
| [foundations.md](guides/foundations.md) | Complete (overview per package) |
| [collections-messaging.md](guides/collections-messaging.md) | Complete |
| [humanizer.md](guides/humanizer.md) | Complete |
| [geography.md](guides/geography.md) | Complete |
| [http.md](guides/http.md) | Complete |
| [mail.md](guides/mail.md) | Complete |
| [io-platform.md](guides/io-platform.md) | Complete |
| [fakers.md](guides/fakers.md) | Complete |

## Remaining documentation gaps

| Item | Notes |
|------|-------|
| `samples/` project | No executable sample app yet — see [TODO](TODO.md) |
| `assets/` PNG icons | Generated — see [assets/README.md](../assets/README.md) |
| Root audit `*.md` files | Archive if still present; not part of user docs |
| Doc site (DocFX / Pages) | Optional |

## Conventions

| Layer | Language | Location |
|-------|----------|----------|
| Package README | English | `src/*/README.md` |
| System guides | English | `docs/guides/*.md` |
| App `.resx` strings | fr / en | Project resources |

## Not published as NuGet

| Project | Role |
|---------|------|
| `MyNet.Observable.Metadata.Generator` | Roslyn analyzer (via `MyNet.Observable`) |
| `*.Tests` | Unit tests — patterns in `tests/` |

## Build & pack

```bash
dotnet build MyNet.slnx
dotnet pack MyNet.slnx -c Release
```

Output: `packages/` — see `build/package.props`.
