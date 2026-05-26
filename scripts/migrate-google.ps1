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

$srcRoot = Join-Path $root "src\MyNet.Utilities\Google\Maps"
$destRoot = Join-Path $root "src\MyNet.Google\Maps"
$repl = @(
    'namespace MyNet\.Utilities\.Google\.Maps', 'namespace MyNet.Google.Maps'
)

Get-ChildItem $srcRoot -Filter *.cs | Group-Object Name | ForEach-Object {
    Copy-Transform $_.Group[0].FullName (Join-Path $destRoot $_.Name) $repl
}

Write-Host "Google migration done."
