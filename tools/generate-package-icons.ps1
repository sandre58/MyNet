# Generates NuGet package icons (128x128 PNG) from SVG sources.
# Usage (repo root): pwsh -File tools/generate-package-icons.ps1
#
# Cross-repo (e.g. MyAvalonia): pass --manifest, --svg-dir, --output after --
# See tools/icon-registry.md

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path $PSScriptRoot -Parent
$project = Join-Path $PSScriptRoot 'MyNet.Tools.PackageIconGenerator\MyNet.Tools.PackageIconGenerator.csproj'

Push-Location $repoRoot
try {
    dotnet run --project $project -c Release
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
