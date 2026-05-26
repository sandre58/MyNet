$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")
$root = (Get-Location).Path

function Copy-Transform {
    param([string]$Src, [string]$Dest, [string[]]$Replacements)
    $dir = Split-Path $Dest -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    $text = [System.IO.File]::ReadAllText($Src)
    for ($i = 0; $i -lt $Replacements.Length; $i += 2) {
        $text = $text -replace $Replacements[$i], $Replacements[$i + 1]
    }
    [System.IO.File]::WriteAllText($Dest, $text)
}

$coreNs = @('namespace MyNet\.Utilities\.Geography', 'namespace MyNet.Geography')
$utilToGeo = @('using MyNet\.Utilities\.Geography', 'using MyNet.Geography', 'Utilities\.Geography\.', 'MyNet.Geography.')

# Core: Utilities/Geography -> MyNet.Geography
$coreDest = Join-Path $root "src\MyNet.Geography"
Get-ChildItem (Join-Path $root "src\MyNet.Utilities\Geography") -Filter *.cs | ForEach-Object {
    Copy-Transform $_.FullName (Join-Path $coreDest $_.Name) $coreNs
}

# Localization: move from old Geography project
$locSrc = Join-Path $root "src\MyNet.Geography\Localization"
$locDest = Join-Path $root "src\MyNet.Geography.Localization\Localization"
if (Test-Path $locSrc) {
    New-Item -ItemType Directory -Force -Path $locDest | Out-Null
    Get-ChildItem $locSrc -File | ForEach-Object {
        $dest = Join-Path $locDest $_.Name
        if ($_.Extension -eq '.cs') {
            Copy-Transform $_.FullName $dest @('MyNet\.Geography\.Localization', 'MyNet.Geography.Localization')
        }
        else { Copy-Item $_.FullName $dest -Force }
    }
}

# CountryDisplayTextStrategy
$strategySrc = Join-Path $root "src\MyNet.Geography\Providers\CountryDisplayTextStrategy.cs"
if (Test-Path $strategySrc) {
    Copy-Transform $strategySrc (Join-Path $root "src\MyNet.Geography.Localization\Providers\CountryDisplayTextStrategy.cs") $utilToGeo
}

# ServiceCollectionExtensions
$extSrc = Join-Path $root "src\MyNet.Geography\Extensions\ServiceCollectionExtensions.cs"
if (Test-Path $extSrc) {
    $repl = $utilToGeo + @(
        'MyNet\.Geography\.Providers\.EmbeddedCountryFlagProvider', 'MyNet.Geography.Resources.EmbeddedCountryFlagProvider',
        'using MyNet\.Geography\.Providers', 'using MyNet.Geography.Localization.Providers; using MyNet.Geography.Resources'
    )
    Copy-Transform $extSrc (Join-Path $root "src\MyNet.Geography.Localization\Extensions\ServiceCollectionExtensions.cs") $repl
}

# EmbeddedCountryFlagProvider -> Resources
$flagSrc = Join-Path $root "src\MyNet.Geography\Providers\EmbeddedCountryFlagProvider.cs"
if (Test-Path $flagSrc) {
    Copy-Transform $flagSrc (Join-Path $root "src\MyNet.Geography.Resources\EmbeddedCountryFlagProvider.cs") @(
        'namespace MyNet\.Geography\.Providers', 'namespace MyNet.Geography.Resources',
        'using MyNet\.Utilities\.Geography', 'using MyNet.Geography',
        'internal sealed class EmbeddedCountryFlagProvider', 'public sealed class EmbeddedCountryFlagProvider',
        'internal abstract class EmbeddedCountryFlagProviderBase', 'public abstract class EmbeddedCountryFlagProviderBase'
    )
}

Write-Host "Geography migration files prepared."
