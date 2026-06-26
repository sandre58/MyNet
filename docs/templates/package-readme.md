# Package README template

Regenerate all package READMEs (MyNet): `powershell -File tools/update-package-readmes.ps1`

Package metadata lives in [`tools/package-readmes.json`](../../tools/package-readmes.json). Sibling repos (e.g. **MyAvalonia**) call the same script with `-RepoRoot`, `-GitHubRepo`, and their own `package-readmes.json`.

---

```markdown
<div align="center">

# {PackageId}

<img src="../../assets/{PackageIcon}" alt="{PackageId}" width="96" height="96" />

{Full package description — complete tagline, one or two sentences.}

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/{PackageId})](https://www.nuget.org/packages/{PackageId})
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **{Feature}** | {Description} |

---

## Installation
...
```

**Notes**

- Badges: Markdown shields in a separate `<div align="center">` (not inside `<p>`).
- `.NET` badge: no `logo=dotnet` (avoids duplicate icon + text).
- NuGet pack rewrites `../../assets/` to the icon file name (`build/package.props`).
