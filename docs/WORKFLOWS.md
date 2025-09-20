# CI/CD Workflows Documentation

This repository uses an **integrated CI/CD approach** with **automatic versioning**, **semantic release analysis**, and **conventional commits integration**:

## 1. 🔄 CI - Build, Test & Version (`.github/workflows/ci.yml`)

**Triggers:**
- Push to `main`, `feature/*`, `hotfix/*` branches
- Pull requests to `main`

**What it does:**
- 🏷️ **Automatic versioning** - Calculates semantic versions per project using conventional commits
- ✅ **Build everything** - All projects with multi-targeting (.NET 8, 9, 10) with computed versions
- ✅ **Run all tests** with code coverage
- ✅ **Generate coverage reports** (HTML, Cobertura, Markdown, Badges)
- ✅ **Package all NuGet packages** with semantic versions (as artifacts)
- ✅ **Deploy coverage to GitHub Pages** (main branch only)
- ✅ **Upload to Codecov** for coverage tracking
- ✅ **PR coverage comments** with diff coverage information

**🎯 Automatic Versioning System:**
- **Semantic analysis**: Uses `@semantic-release/commit-analyzer` to parse conventional commits
- **Per-project versioning**: Each project gets its own calculated version based on its changes
- **Branch-specific suffixes**: 
  - `main` branch: `X.Y.Z-pre.BUILD` format
  - Other branches: `X.Y.Z-beta.BUILD` format
- **Tag-based detection**: Returns exact version if current commit is tagged
- **Smart change detection**: Only increments version when project has actual changes

**Artifacts produced:**
- `coverage-report` - HTML coverage report with badges
- `packages` - All .nupkg files with semantic versions ready for distribution

**Version calculation process:**
1. **Project detection**: Scans all `src/*/` directories
2. **Commit analysis**: Uses semantic-release to analyze conventional commits since last tag
3. **Version computation**: Calculates next semantic version (patch/minor/major)
4. **Format application**: Applies branch-specific suffix (`-pre` or `-beta`)
5. **Build integration**: Injects computed versions into `dotnet build` and `dotnet pack`

**Coverage features:**
- **GitHub Pages**: Deployed automatically on main branch
- **Codecov integration**: Upload for detailed coverage tracking  
- **PR comments**: Coverage diff and summary in pull requests
- **Multi-format reports**: HTML, Cobertura, Markdown, Badges

**Coverage deployment:**
- Only on `main` branch pushes
- Available at: `https://sandre58.github.io/MyNet/`

---

## 2. 📊 Version Calculation Details

**The `scripts/compute-version.js` script:**

1. **Tag Detection**: Checks if current commit has an existing project tag
2. **Last Version**: Finds the last tag for the project (`ProjectName/vX.Y.Z`)
3. **Commit Analysis**: Extracts commits since last tag affecting the project directory
4. **Semantic Analysis**: Uses `@semantic-release/commit-analyzer` with conventional commits preset
5. **Version Increment**: Determines release type (patch/minor/major) and increments
6. **Branch Suffix**: Applies `-pre` (main) or `-beta` (other branches) suffix

**Version Calculation Examples:**

```bash
# Starting from MyNet.Utilities/v1.4.0

# Patch increment (fix commits)
git commit -m "fix(utils): resolve null reference exception"
# Result: 1.4.1-pre.X (main) or 1.4.1-beta.X (feature branch)

# Minor increment (feat commits)  
git commit -m "feat(utils): add new string extension methods"
# Result: 1.5.0-pre.X (main) or 1.5.0-beta.X (feature branch)

# Major increment (breaking changes)
git commit -m "feat(utils)!: redesign configuration API

BREAKING CHANGE: Configuration class constructor signature changed"
# Result: 2.0.0-pre.X (main) or 2.0.0-beta.X (feature branch)

# No increment (docs, style, etc.)
git commit -m "docs(utils): update README examples"  
# Result: 1.4.0 (no change, last tag version)
```

**🔧 Integration Points:**
- **Build Step**: Versions injected via `-p:Version=$version` parameters
- **Pack Step**: Versions applied via `-p:PackageVersion=$version` parameters  
- **Artifacts**: All packages include computed semantic versions in filenames
- **Per-Project**: Each project in `src/` gets independent version calculation

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

2. **CI automatically runs on feature branch**:
   - 🏷️ **Calculates version**: `MyNet.Utilities` → `1.5.0-beta.123` (beta suffix for feature branch)
   - ✅ **Builds with version**: All projects built with their computed semantic versions
   - ✅ **Runs tests** with coverage collection
   - ✅ **Creates NuGet packages** with `-beta` versions as artifacts
   - ❌ **No deployment** (not main branch)

3. **Create Pull Request**:
   - ✅ CI runs again with PR-specific features
   - ✅ **Coverage comment added to PR** with diff coverage
   - ✅ **Version preview**: Shows what versions will be used when merged
   - ✅ Review process with automated checks
   - ✅ Merge to `main` after approval

4. **Main branch CI** (post-merge):
   - 🏷️ **Recalculates versions**: `MyNet.Utilities` → `1.5.0-pre.124` (pre suffix for main)
   - ✅ **Builds with production versions**: All projects built with `-pre` versions  
   - ✅ **Full CI pipeline** runs automatically
   - ✅ **Coverage deployed to GitHub Pages** (`https://sandre58.github.io/MyNet/`)
   - ✅ **Packages ready for release**: All artifacts available with computed versions
   - ✅ **Codecov integration** for coverage tracking

5. **Ready for distribution**:
   ```bash
   # Packages are automatically generated with semantic versions:
   # MyNet.Utilities.1.5.0-pre.124.nupkg (ready for pre-release)
   # MyNet.Observable.2.3.1-pre.124.nupkg (if it had changes too)
   ```

**🎯 Version Calculation Examples:**

Based on conventional commits, the system automatically determines version increments:

```bash
# Patch version (1.4.0 → 1.4.1-pre.X)
git commit -m "fix(cache): resolve memory leak in TTL cleanup"

# Minor version (1.4.0 → 1.5.0-pre.X)  
git commit -m "feat(cache): add distributed caching support"

# Major version (1.4.0 → 2.0.0-pre.X)
git commit -m "feat(cache)!: redesign caching API

BREAKING CHANGE: CacheManager constructor signature changed"
```

**🔄 Continuous Integration Benefits:**
- **No manual version management**: Versions computed automatically from commits
- **Per-project independence**: Each project versioned separately based on its changes  
- **Branch-aware versioning**: Different suffixes for different branches
- **Ready-to-ship packages**: All packages built with proper semantic versions
- **Consistent numbering**: Semantic versioning enforced through conventional commits

---

## 🔧 Configuration

### Automatic Versioning Setup

**Required files:**
- `scripts/compute-version.js` - Semantic version calculation script
- `package.json` - Node.js dependencies for semantic-release tools
- `.github/workflows/ci.yml` - CI workflow with integrated versioning

**Dependencies (auto-installed):**
```json
{
  "type": "module",
  "dependencies": {
    "@semantic-release/commit-analyzer": "^13.0.0",
    "conventional-changelog-conventionalcommits": "^8.0.0", 
    "semver": "^7.6.3"
  }
}
```

### Required Secrets

| Secret | Purpose | Required | Notes |
|--------|---------|----------|-------|
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

The CI workflow is configured with required permissions:
- **CI**: `contents: write`, `pages: write`, `id-token: write`, `checks: write`, `pull-requests: write`, `actions: read`

### Conventional Commits Configuration

**Semantic versioning rules:**
- **patch**: `fix:` commits increment patch version (1.0.0 → 1.0.1)
- **minor**: `feat:` commits increment minor version (1.0.0 → 1.1.0)
- **major**: Breaking changes (`!` or `BREAKING CHANGE:`) increment major version (1.0.0 → 2.0.0)

**Supported commit types:**
- `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`
- **Breaking changes**: `feat!:` or `BREAKING CHANGE:` in commit footer
- **Scopes**: `auth`, `utils`, `http`, `observable`, `ui`, `wpf`, `avalonia`, etc.

**Version format:**
- **Main branch**: `X.Y.Z-pre.BUILD` (e.g., `1.5.0-pre.124`)
- **Feature/other branches**: `X.Y.Z-beta.BUILD` (e.g., `1.5.0-beta.89`)

---

## 🎛️ Customization

### Adding new projects

New projects in `src/` are automatically detected by the versioning system. Ensure they:
- Have a `.csproj` file with appropriate metadata
- Are packable (`<IsPackable>true</IsPackable>` or omit for default true)
- Follow conventional commit patterns for automatic versioning
- Use project-scoped commits: `feat(projectname): description` for targeted versioning

### Modifying CI pipeline

**Edit `.github/workflows/ci.yml`**:
- **Add/remove .NET versions**: Update `dotnet-version` matrix
- **Modify versioning**: Update `scripts/compute-version.js` script  
- **Modify build parameters**: Adjust `dotnet build` command options with version injection
- **Platform-specific builds**: Update workload installations
- **Coverage configuration**: Modify ReportGenerator settings
- **Package versioning**: Modify version application in build and pack steps

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

**1. Version calculation issues**
- **Problem**: Incorrect versions calculated or script failures
- **Solution**: Check conventional commit format and `compute-version.js` script
- **Check**: Review commit messages for proper `feat:`, `fix:`, etc. prefixes
- **Debug**: Run `node scripts/compute-version.js <ProjectName>` locally

**2. No version changes detected**  
- **Problem**: All projects get same version despite different changes
- **Solution**: Use project-scoped conventional commits: `feat(utilities): new feature`
- **Check**: Ensure commits affect files in specific project directories
- **Example**: `git log --oneline -- src/MyNet.Utilities/` to see project-specific commits

**3. Node.js/npm dependencies issues**
- **Problem**: `@semantic-release/commit-analyzer` not found or import errors  
- **Solution**: Ensure `package.json` has `"type": "module"` and correct dependencies
- **Check**: Verify Node.js 20 is used and npm install runs successfully
- **Debug**: Run `npm install` locally and test `node scripts/compute-version.js`

**4. Coverage not deploying to GitHub Pages**
- **Problem**: Coverage report not accessible via GitHub Pages
- **Solution**: Enable GitHub Pages with "GitHub Actions" source
- **Check**: Workflow has `pages: write` permission
- **Verify**: Main branch push triggered the deployment

**5. Semantic-release analysis failures**
- **Problem**: Version calculation script fails to analyze commits
- **Solution**: Use conventional commit format (`type(scope): description`)
- **Check**: Ensure commits follow `feat:`, `fix:`, `docs:` etc. patterns
- **Debug**: Test semantic-release locally with conventional commits

**6. Build version injection failures**
- **Problem**: Packages built without computed versions
- **Solution**: Check version parsing and JSON output in CI logs
- **Check**: Verify `steps.versions.outputs.projects_versions` contains valid JSON  
- **Debug**: Look for version calculation step output in GitHub Actions logs

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
# Test version calculation script
node scripts/compute-version.js MyNet.Utilities

# Test semantic-release commit analysis
npm install
node -e "import('@semantic-release/commit-analyzer').then(console.log)"

# Test project detection  
find src -name "*.csproj" -not -path "*/Demo*" -not -path "*/Test*"

# Test conventional commits parsing
git log --oneline --grep="^feat\|^fix\|^docs" -- src/MyNet.Utilities/
```

**4. Validate project structure**
```
src/
├── MyNet.Utilities/
│   ├── MyNet.Utilities.csproj    ← Detected automatically
│   └── ... (project files)
scripts/
├── compute-version.js            ← Version calculation script
package.json                      ← Node.js dependencies
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
- **Semantic Release**: [@semantic-release/commit-analyzer](https://github.com/semantic-release/commit-analyzer)
- **Conventional Commits**: [Specification](https://conventionalcommits.org/)
- **Semver**: [Semantic Versioning](https://semver.org/)
- **Project-specific**: Check `scripts/compute-version.js` for version calculation logic
