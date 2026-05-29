# Security Policy

## Supported Versions

Security fixes are provided for packages published from the **`main`** branch and the **latest tagged release** on [NuGet](https://www.nuget.org/packages?q=MyNet).

| Version | Supported |
| ------- | --------- |
| Latest release (`v*` tag) | Yes |
| `main` (pre-release packages) | Yes |
| Older major/minor releases | Best effort only |

If you depend on an older version, upgrade to the latest release when possible before reporting an issue.

## Reporting a Vulnerability

**Please do not open a public GitHub issue for security vulnerabilities.**

Instead, report them privately to:

**andre.cs2i@gmail.com**

Include as much detail as you can:

- Affected package(s) and version(s)
- Steps to reproduce or a minimal proof of concept
- Impact assessment (data exposure, remote code execution, denial of service, etc.)
- Any suggested fix, if you have one

### What to expect

| Step | Target timeline |
| ---- | ---------------- |
| Acknowledgement | Within 5 business days |
| Initial assessment | Within 10 business days |
| Fix or mitigation plan | Depends on severity; critical issues are prioritized |

We will coordinate disclosure with you before publishing a security advisory or release notes that reference your report.

## Out of Scope

The following are generally **not** treated as security vulnerabilities in this project:

- Issues in dependencies already fixed in a newer release listed in `Directory.Packages.props` (use Dependabot PRs or upgrade manually)
- Missing features or hardening that is not a demonstrated vulnerability
- Reports requiring physical access to an unlocked machine running a consumer app that references MyNet libraries

Thank you for helping keep MyNet and its users safe.
