# Documentation & release backlog

**Last updated:** 2026-05-29

See [documentation index](index.md) for the full map.

---

## Recently completed

- [x] System guides expanded: foundations, humanizer, fakers, http, mail, io-platform, geography, collections-messaging
- [x] New UI guides: [dialogs.md](guides/dialogs.md), [notifications-and-toasts.md](guides/notifications-and-toasts.md), [shell.md](guides/shell.md)
- [x] Merged legacy docs into observable / ui / globalization guides
- [x] 23 package READMEs + NuGet metadata on `.csproj`
- [x] `docs/index.md`, `getting-started.md`, `templates/package-readme.md`

---

## P0 — Polish

- [ ] Fix remaining `github.com/sandre58/MyNet` links in package README badges (search repo)
- [x] Add `assets/*.png` icons referenced by `PackageIcon` — regenerate with `tools/generate-package-icons.ps1`
- [ ] `samples/MinimalDesktop` — console or DI host demonstrating Observable + optional UI services
- [ ] Verify single `INotificationService` story when both `AddNotifications` and `AddToasting` are used (document or adjust DI)

---

## P1 — NuGet & CI

- [ ] `CHANGELOG.md` entry for documentation / packaging milestone
- [ ] CI: `dotnet build` + `dotnet test` + `dotnet pack` on `MyNet.slnx`
- [ ] Publish workflow (optional)

---

## P2 — Optional

- [ ] DocFX / GitHub Pages from `docs/` + XML docs
- [ ] Integrate `DialogSystem_Review` content if design notes are recovered (dialogs guide is API-focused today)
- [x] Theme service guide (`IThemeService`) — [guides/theming.md](guides/theming.md)
- [ ] `CONTRIBUTING.md` — add link to `docs/index.md`

---

## P3 — Product backlog (non-doc)

Observable behaviors (if still planned): LoggingBehavior, UndoRedoBehavior, UIThreadMarshallingBehavior — document in [observable.md](guides/observable.md) when implemented.

---

## Commands

```bash
dotnet restore
dotnet build MyNet.slnx -c Release
dotnet test MyNet.slnx -c Release
dotnet pack MyNet.slnx -c Release
```
