# CI/CD Workflows Documentation

This repository uses a **3-workflow approach** for Continuous Integration and Deployment with **automatic changelog generation** and **conventional commits integration**:

## 1. 🔄 CI - Build and Test (`.github/workflows/ci.yml`)

**Triggers:**
- Push to `main`, `feature/*`, `hotfix/*` branches
- Pull requests to `main`

**What it does:**
- ✅ **Build everything** - All projects with multi-targeting (.NET 8, 9, 10)
- ✅ **Run all tests** with code coverage
- ✅ **Generate coverage reports** (HTML, Cobertura, Markdown, Badges)
- ✅ **Package all NuGet packages** (as artifacts)
- ✅ **Build all demos** (WPF, Avalonia Desktop, Avalonia Browser)
- ✅ **Deploy coverage to GitHub Pages** (main branch only)
- ✅ **Upload to Codecov** for coverage tracking
- ✅ **PR coverage comments** with diff coverage information

**Artifacts produced:**
- `coverage-report` - HTML coverage report with badges
- `nuget-packages` - All .nupkg files ready for distribution
- `demos` - Built demo applications (WPF, Avalonia Desktop, Browser)

**Coverage features:**
- **GitHub Pages**: Deployed automatically on main branch
- **Codecov integration**: Upload for detailed coverage tracking
- **PR comments**: Coverage diff and summary in pull requests
- **Multi-format reports**: HTML, Cobertura, Markdown, Badges

**Coverage deployment:**
- Only on `main` branch pushes
- Available at: `https://sandre58.github.io/MyNet/`

---

## 2. 🏷️ Manual - Tag Projects (`.github/workflows/manual-tag.yml`)

**Triggers:**
- Manual execution via GitHub Actions UI

**Parameters:**
- `dry_run` (default: true) - Preview what would be tagged without creating tags
- `force_all` (default: false) - Force tag all projects regardless of changes

**What it does:**
- 🔍 **Detect all packable projects** (excludes demos and tests)
- 📊 **Analyze changes** since last tag for each project
- 🏷️ **Create project-specific tags** in format: `ProjectName/vX.Y.Z`
- ⚡ **Smart change detection** - Only tags projects with actual changes
- 📝 **Comprehensive reporting** of what will be/was tagged

**Tag format:**
```
MyNet.Utilities/v1.2.3
MyNet.Observable/v2.1.0
MyNet.Http/v3.0.1
```

**Workflow:**
1. Run with `dry_run: true` to see what would be tagged
2. Review the output
3. Run with `dry_run: false` to create the actual tags

---

## 3. 🚀 Release - Publish Package (`.github/workflows/release-tag.yml`)

**Triggers:**
- Push of tags matching pattern `*/v*` (e.g., `MyNet.Utilities/v1.2.3`)

**What it does:**
- 🎯 **Parse tag** to extract project name and version
- 🔍 **Find specific project** file and verify version consistency
- 🏗️ **Build only the tagged project** (efficient, targeted build)
- 🧪 **Run tests** for the specific project (if test project exists)
- 📦 **Pack NuGet package** with symbols and source code
- 🌐 **Publish to NuGet.org** (if `NUGET_API_KEY` secret configured)
- 📚 **Publish to GitHub Packages** (always, for backup/private access)
- 📝 **Generate dual changelogs** using git-chglog with conventional commits
- 📄 **Update project CHANGELOG.md** (permanent project documentation)
- 🎉 **Create GitHub Release** with release changelog and packages
- 📊 **Upload artifacts** for download and backup

**Changelog Features:**
- **🔄 Dual changelog approach**:
  - **Release changelog**: Focused content for GitHub Release (changes between tags)
  - **Project changelog**: Complete history in `{ProjectFolder}/CHANGELOG.md`
- **📋 git-chglog integration**: Professional changelog generation from conventional commits
- **🏷️ Conventional commits parsing**: Automatic categorization (Features, Bug Fixes, etc.)
- **🔗 Smart linking**: Automatic links to commits and repository
- **⚠️ Breaking changes detection**: Special handling for BREAKING CHANGE commits
- **📝 Automatic commit**: Updated CHANGELOG.md pushed back to main branch

**Publications:**
- **NuGet.org**: Main package distribution (public packages)
- **GitHub Packages**: Backup/private distribution (always published)
- **GitHub Releases**: With comprehensive changelog and attached packages

**Release features:**
- **Smart changelog generation** from conventional commits
- **Package files attached** to release (NuGet packages)
- **Pre-release detection** (versions containing `-` like `1.0.0-alpha`)
- **Automatic release notes** (GitHub's built-in feature + custom changelog)
- **Project documentation update** (CHANGELOG.md files maintained automatically)

---

## 🎯 Complete Workflow Example

### Scenario: You modified `MyNet.Utilities` and want to release it

1. **Development** (using conventional commits):
   ```bash
   # Work on your feature branch
   git checkout -b feature/add-caching-support
   
   # Make changes with conventional commit messages
   git commit -m "feat(cache): add memory caching with TTL support"
   git commit -m "test(cache): add comprehensive caching tests"
   git commit -m "docs(cache): add caching documentation and examples"
   
   git push origin feature/add-caching-support
   ```

2. **CI automatically runs**:
   - ✅ Builds everything (all projects, all targets)
   - ✅ Runs tests with coverage collection
   - ✅ Creates NuGet packages as artifacts
   - ✅ Builds demo applications
   - ❌ No deployment (not main branch)

3. **Create Pull Request**:
   - ✅ CI runs again with PR-specific features
   - ✅ **Coverage comment added to PR** with diff coverage
   - ✅ Review process with automated checks
   - ✅ Merge to `main` after approval

4. **Main branch CI**:
   - ✅ Full CI pipeline runs automatically
   - ✅ **Coverage deployed to GitHub Pages** (`https://sandre58.github.io/MyNet/`)
   - ✅ All artifacts available for download
   - ✅ Codecov integration for coverage tracking

5. **Create tags manually** (smart detection):
   ```
   Go to Actions → Manual - Tag Projects → Run workflow
   
   Step 1: Set dry_run: true
   Step 2: Click "Run workflow" 
   Step 3: Review output (e.g., "MyNet.Utilities will be tagged as v2.1.0")
   Step 4: Run again with dry_run: false to create actual tags
   ```

6. **Automatic release** (triggered by tags):
   - 🎯 Tag `MyNet.Utilities/v2.1.0` triggers release workflow
   - 🏗️ Builds and tests only MyNet.Utilities project
   - 📦 Packs and publishes NuGet package
   - 📝 **Generates professional changelog** using git-chglog:
     ```markdown
     # MyNet.Utilities v2.1.0
     
     ## 🚀 Features
     - **cache:** add memory caching with TTL support ([a1b2c3d])
     
     ## 📚 Documentation  
     - **cache:** add caching documentation and examples ([e4f5g6h])
     
     ## ✅ Tests
     - **cache:** add comprehensive caching tests ([i7j8k9l])
     ```
   - 📄 **Updates `src/MyNet.Utilities/CHANGELOG.md`** with complete project history
   - 🎉 **Creates GitHub Release** with changelog and attached packages
   - 🔄 **Commits CHANGELOG.md back to main** automatically

---

## 🔧 Configuration

### Required Secrets

| Secret | Purpose | Required | Notes |
|--------|---------|----------|-------|
| `NUGET_API_KEY` | Publish to NuGet.org | Optional | Get from nuget.org profile |
| `CODECOV_TOKEN` | Upload to Codecov | Optional | Enhanced coverage reporting |

### Repository Settings

**GitHub Pages:**
1. Go to Settings → Pages
2. Set Source to "GitHub Actions"
3. Coverage will be available at `https://sandre58.github.io/MyNet/`

**Branch Protection (Recommended):**
- Require PR reviews for `main` branch
- Require status checks (CI workflow)
- Require up-to-date branches before merging

### Workflow Permissions

The workflows are configured with minimal required permissions:
- **CI**: `contents: read`, `pages: write`, `id-token: write`
- **Manual Tag**: `contents: write` (to create tags and push CHANGELOG.md)
- **Release**: `contents: write`, `packages: write` (to create releases and publish)

### Changelog Configuration

**git-chglog setup:**
- **Full changelogs**: `.chglog/config.yml` + `CHANGELOG.tpl.md`
- **Release changelogs**: `.chglog/release-config.yml` + `RELEASE.tpl.md`
- **Conventional commits**: Automatic parsing and categorization
- **Project changelogs**: Updated in `src/{ProjectName}/CHANGELOG.md`

**Supported commit types:**
- `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`
- **Breaking changes**: `BREAKING CHANGE:` in commit footer
- **Scopes**: `auth`, `utils`, `http`, `observable`, `ui`, `wpf`, `avalonia`, etc.

---

## 🎛️ Customization

### Adding new projects

New projects in `src/` are automatically detected. Ensure they:
- Have a `.csproj` file with appropriate metadata
- Are packable (`<IsPackable>true</IsPackable>` or omit for default true)
- Have a `version.json` file for Nerdbank.GitVersioning
- Follow conventional commit patterns for changelog generation

### Modifying CI pipeline

**Edit `.github/workflows/ci.yml`**:
- **Add/remove .NET versions**: Update `dotnet-version` matrix
- **Modify build parameters**: Adjust `dotnet build` command options
- **Platform-specific builds**: Update workload installations
- **Coverage configuration**: Modify ReportGenerator settings
- **Demo projects**: Add/remove demo build steps

### Modifying release pipeline

**Edit `.github/workflows/release-tag.yml`**:
- **NuGet publishing**: Modify `dotnet nuget push` parameters
- **Changelog generation**: Update git-chglog configuration
- **Release content**: Customize GitHub release creation
- **Artifact handling**: Modify package and upload steps

### Custom changelog templates

**Create custom templates in `.chglog/`**:
```yaml
# .chglog/custom-config.yml
style: github
template: CUSTOM.tpl.md
info:
  title: MyProject
  repository_url: https://github.com/sandre58/MyNet
# ... custom configuration
```

### Tag pattern customization

**Modify tag trigger patterns**:
```yaml
# In release-tag.yml
on:
  push:
    tags:
      - 'v*'           # Simple version tags
      - 'release/*'    # Release branch tags  
      - 'custom-*'     # Custom pattern
```

### Conventional commit customization

**Update `.chglog/config.yml` for custom commit types**:
```yaml
commit_groups:
  title_maps:
    feat: "🚀 Features"
    fix: "🐛 Bug Fixes"
    custom: "🎨 Custom Changes"  # Add custom types
```

---

## 🐛 Troubleshooting

### Common Issues

**1. Version mismatch in release workflow**
- **Problem**: "Tag version (1.2.3) does not match project version (1.2.4)"
- **Solution**: Ensure the tag version matches NBGV calculated version
- **Check**: Review `version.json` configuration and commit history
- **Debug**: Run `nbgv get-version` in project directory

**2. Project file not found**
- **Problem**: "Project file not found for: MyNet.SomeProject"
- **Solution**: Verify project name in tag matches `.csproj` filename exactly
- **Check**: Ensure project is in `src/` directory structure
- **Example**: Tag `MyNet.Utilities/v1.0.0` expects `src/MyNet.Utilities/MyNet.Utilities.csproj`

**3. NuGet publish failures**
- **Problem**: Package already exists or authentication failed
- **Solution**: Check `NUGET_API_KEY` secret configuration
- **Alternative**: Version conflicts (package version already published)
- **Debug**: Check NuGet.org for existing package versions

**4. Coverage not deploying to GitHub Pages**
- **Problem**: Coverage report not accessible via GitHub Pages
- **Solution**: Enable GitHub Pages with "GitHub Actions" source
- **Check**: Workflow has `pages: write` permission
- **Verify**: Main branch push triggered the deployment

**5. Changelog generation issues**
- **Problem**: Empty or malformed changelogs
- **Solution**: Use conventional commit format (`type(scope): description`)
- **Check**: git-chglog configuration in `.chglog/` directory
- **Debug**: Test git-chglog locally with `git-chglog --dry-run`

**6. Manual tagging workflow not detecting changes**
- **Problem**: "No projects need to be tagged" when changes exist
- **Solution**: Ensure commits affect files in project directories
- **Check**: Git history with `git log --oneline -- src/ProjectName/`
- **Alternative**: Use `force_all: true` to tag all projects

### Debug Steps

**1. Check workflow logs**
- Go to Actions tab in GitHub repository
- Select failed workflow run
- Expand job steps to see detailed logs
- Look for specific error messages

**2. Verify repository configuration**
- **Secrets**: Check NUGET_API_KEY and CODECOV_TOKEN
- **Permissions**: Ensure workflows have required permissions
- **Branch protection**: Verify rules don't block automation

**3. Test locally**
```bash
# Test NBGV version calculation
nbgv get-version

# Test git-chglog generation  
git-chglog --config .chglog/config.yml --dry-run

# Test project detection
find src -name "*.csproj" -not -path "*/Demo*" -not -path "*/Test*"
```

**4. Validate project structure**
```
src/
├── MyNet.Utilities/
│   ├── MyNet.Utilities.csproj    ← Must match tag name
│   ├── version.json              ← NBGV configuration
│   └── CHANGELOG.md              ← Updated automatically
```

**5. Check conventional commits**
```bash
# Valid conventional commit examples
git log --oneline --grep="^feat\|^fix\|^docs"

# Check for breaking changes
git log --grep="BREAKING CHANGE"
```

### Getting Help

- **Workflow documentation**: [GitHub Actions Docs](https://docs.github.com/en/actions)
- **Nerdbank.GitVersioning**: [Official Documentation](https://github.com/dotnet/Nerdbank.GitVersioning)
- **git-chglog**: [Configuration Guide](https://github.com/git-chglog/git-chglog)
- **Conventional Commits**: [Specification](https://conventionalcommits.org/)
- **Project-specific**: Check `.github/docs/` for additional documentation
