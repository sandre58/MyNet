# Generates NuGet package icons (128x128 PNG) from SVG sources.
# Usage (repo root): pwsh -File tools/generate-package-icons.ps1

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
