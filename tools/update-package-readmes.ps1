# Updates src/*/README.md with icon, badges, features, and copyright.
# Usage (MyNet repo root):
#   powershell -File tools/update-package-readmes.ps1
# Usage (sibling repo, e.g. MyAvalonia):
#   powershell -File tools/update-package-readmes.ps1
#     -RepoRoot ..\\MyAvalonia
#     -GitHubRepo sandre58/MyAvalonia
#     -PackagesConfig ..\\MyAvalonia\\tools\\package-readmes.json
param(
    [string]$RepoRoot,
    [string]$GitHubRepo = 'sandre58/MyNet',
    [string]$PackagesConfig
)

$ErrorActionPreference = 'Stop'

if (-not $RepoRoot) {
    $RepoRoot = Split-Path $PSScriptRoot -Parent
}
if (-not $PackagesConfig) {
    $PackagesConfig = Join-Path $PSScriptRoot 'package-readmes.json'
}
if (-not (Test-Path $PackagesConfig)) {
    throw "Packages config not found: $PackagesConfig"
}

$config = Get-Content $PackagesConfig -Raw -Encoding UTF8 | ConvertFrom-Json
if (-not $config.githubRepo) {
    $config | Add-Member -NotePropertyName githubRepo -NotePropertyValue $GitHubRepo -Force
}
$GitHubRepo = [string]$config.githubRepo
if ([string]::IsNullOrWhiteSpace($GitHubRepo)) {
    $GitHubRepo = 'sandre58/MyNet'
}

$packages = @(
    foreach ($entry in $config.packages) {
        @{
            Id       = [string]$entry.id
            Icon     = [string]$entry.icon
            Tagline  = [string]$entry.tagline
            Features = @([string[]]$entry.features)
        }
    }
)

$year = (Get-Date).Year
$copyright = ('Copyright {0} 2016-{1} - St{2}phane ANDRE. All Rights Reserved.' -f [char]0x00A9, $year, [char]0x00E9)
$licenseUrl = "https://github.com/$GitHubRepo/blob/main/LICENSE"

function Get-Summary([string]$Tagline) {
    $summary = $Tagline.Trim()
    if ($summary -notmatch '[.!?]$') {
        $summary += '.'
    }
    return $summary
}

function Format-FeatureRow([string]$line) {
    if ($line -match '^(.+?)\|(.+)$') {
        return "| **$($matches[1].Trim())** | $($matches[2].Trim()) |"
    }
    return "| | $line |"
}

function Format-PackageBody([string]$body) {
    $body = [regex]::Replace($body.Trim(), '(?:\r?\n)---\s*(?=\r?\n)', "`n")
    # Legacy bullet-list Features section (replaced by header table)
    $body = [regex]::Replace($body, '(?s)\r?\n## Features\r?\n.*?(?=\r?\n## )', "`n")
    if ($body -match '## Related packages') {
        $body = $body -replace '(\r?\n)(## Related packages)', "`$1---`$1`$2"
    }
    if ($body -match '## Related MyNet packages') {
        $body = $body -replace '(\r?\n)(## Related MyNet packages)', "`$1---`$1`$2"
    }
    if ($body -match '## Documentation') {
        $body = $body -replace '(\r?\n)(## Documentation)', "`$1---`$1`$2"
    }
    return $body.Trim()
}

function New-PackageHeader {
    param($Id, $Tagline, $Icon, $Features, [string]$GitHubRepository)
    $summary = Get-Summary $Tagline
    $featureRows = ($Features | ForEach-Object { Format-FeatureRow $_ }) -join "`n"
    @"
<div align="center">

# $Id

<img src="../../assets/$Icon" alt="$Id" width="96" height="96" />

*$summary*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/$GitHubRepository)]($licenseUrl)
[![NuGet](https://img.shields.io/nuget/v/${Id})](https://www.nuget.org/packages/${Id})
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
$featureRows

"@
}

function Get-PackageBody {
    param([string]$ReadmePath)
    $utf8 = New-Object System.Text.UTF8Encoding $false
    if (-not (Test-Path $ReadmePath)) { return $null }
    $raw = [System.IO.File]::ReadAllText($ReadmePath, $utf8)
    $markers = @('## Installation', '## Reference')
    $idx = -1
    foreach ($m in $markers) {
        $idx = $raw.IndexOf($m)
        if ($idx -ge 0) { break }
    }
    if ($idx -lt 0) { return $null }
    $body = $raw.Substring($idx).Trim()
    if ($body.StartsWith('## Reference')) {
        $body = '## Installation' + $body.Substring('## Reference'.Length)
    }
    $body = $body -replace '(?s)\r?\n## License\r?\n.*$', ''
    $body = $body -replace '(?s)\r?\n## Copyright\r?\n.*$', ''
    $body = $body -replace '(?s)\r?\n---\r?\n\r?\n<div align="center">.*$', ''
    $body = $body -replace '^(?:---\s*)+', ''
    return $body.Trim()
}

function New-PackageFooter {
    param([string]$CopyrightText, [string]$GitHubRepository)
    @"

---

<div align="center">

<sub>

$CopyrightText

<br/>

Released under the [MIT License]($licenseUrl).

</sub>

</div>
"@
}

foreach ($pkg in $packages) {
    $readmePath = Join-Path $RepoRoot "src\$($pkg.Id)\README.md"
    if (-not (Test-Path $readmePath)) {
        Write-Warning "Missing README: $readmePath"
        continue
    }

    $body = Get-PackageBody -ReadmePath $readmePath
    if (-not $body) {
        Write-Warning "No ## Installation (or ## Reference) in $readmePath"
        continue
    }
    $body = Format-PackageBody $body

    $header = New-PackageHeader @pkg -GitHubRepository $GitHubRepo
    $footer = New-PackageFooter -CopyrightText $copyright -GitHubRepository $GitHubRepo
    $newContent = $header.TrimEnd() + "`n`n---`n`n" + $body + $footer + "`n"
    $newContent = [regex]::Replace($newContent, '(\r?\n)(?:---\s*){2,}(?=\r?\n)', "`$1---`$1")
    if (-not $newContent.EndsWith("`n")) { $newContent += "`n" }

    $utf8 = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($readmePath, $newContent, $utf8)
    Write-Host "Updated $($pkg.Id)"
}

Write-Host "Done."
