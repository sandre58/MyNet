# Contributing to MyNet

Thank you for your interest in contributing to **MyNet** (My .NET). This monorepo publishes modular NuGet packages for MVVM desktop applications on **.NET 10**.

## Before you start

- Read the [documentation index](docs/index.md) and [getting started](docs/getting-started.md) guide.
- Search [existing issues](https://github.com/sandre58/MyNet/issues) to avoid duplicates.
- **Security vulnerabilities:** follow [SECURITY.md](SECURITY.md) — do not open a public issue.

## How to contribute

### 1. Reporting issues

Use GitHub **Issues** with the provided templates:

| Template | Use when |
| -------- | -------- |
| **Bug report** | Something behaves incorrectly |
| **Feature request** | You need new API or behavior |
| **Question** | Usage or integration help |

Public issues are for bugs, features, and questions only — not for security reports.

### 2. Development setup

**Prerequisites**

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (preview builds are expected; see `global.json`)
- Git

**Build and test**

```bash
git clone https://github.com/sandre58/MyNet.git
cd MyNet
dotnet restore
dotnet build
dotnet test
```

Coverage (optional, matches CI):

```bash
dotnet test /p:CollectCoverage=true
```

### 3. Submitting pull requests

1. Fork the repository and create a branch from `main`.
2. Use a descriptive branch name, for example:
   - `feature/observable-validation-messages`
   - `bugfix/ui-dialog-null-ref`
3. Keep changes focused — one concern per PR when possible.
4. Run `dotnet build` and `dotnet test` locally before pushing.
5. Fill in the [pull request template](.github/PULL_REQUEST_TEMPLATE.md).

**Commit messages:** prefer [Conventional Commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `docs:`, `test:`, `refactor:`, …).

### 4. Coding standards

- Target **`net10.0`** with **`LangVersion` preview** — use modern C# features where they improve clarity.
- Enable nullable reference types (project default).
- Follow [.editorconfig](.editorconfig) and [StyleCop](stylecop.json) rules enforced at build time.
- Document public APIs with XML comments (`GenerateDocumentationFile` is enabled).
- Keep types and methods focused; match patterns in the surrounding package.

### 5. Dependencies

Versions are centralized in [`Directory.Packages.props`](Directory.Packages.props) (Central Package Management).

- To add a dependency: add a `PackageVersion` entry, then reference the package in the `.csproj` **without** a version attribute.
- Shared test dependencies belong in [`build/dependencies.props`](build/dependencies.props).
- Analyzer packages belong in [`build/code-analysis.props`](build/code-analysis.props).

Do not pin versions inside individual `.csproj` files unless there is a documented exception.

### 6. Testing

- Add or update unit tests in the matching `tests/MyNet.*.Tests` project.
- Cover the behavior you change — especially edge cases and regressions.
- CI runs the full test suite and enforces coverage thresholds on critical assemblies.

### 7. Documentation

Update documentation when you change public API or user-visible behavior:

- Package `README.md` under `src/MyNet.*/`
- System guides under `docs/guides/`
- [`CHANGELOG.md`](CHANGELOG.md) is updated at release time from tags and commits

### 8. Pull request review

- PRs are reviewed as soon as possible.
- You may be asked to adjust design, tests, or docs before merge.
- Be responsive to feedback to keep the review cycle short.

## Code of conduct

By contributing, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

## Getting help

- Open a **Question** issue for usage topics.
- Email **andre.cs2i@gmail.com** for security reports or private coordination.

---

Thank you for helping make MyNet better.
