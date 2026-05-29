#Requires -Version 5.1
<#
.SYNOPSIS
    Verifies that each tested MyNet assembly meets the configured line coverage threshold.

.DESCRIPTION
    Reads Cobertura files under TestResults/coverage/{Project}.Tests/ and checks the primary
    assembly (MyNet.X.Tests -> MyNet.X) against assembly-thresholds.json.
#>
param(
    [string] $CoverageRoot = '',

    [string] $TestsRoot = '',

    [string] $ThresholdsFile = '',

    [switch] $FailOnMissingCoverage
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

if ([string]::IsNullOrWhiteSpace($CoverageRoot)) {
    $CoverageRoot = Join-Path $repoRoot 'TestResults\coverage'
}

if ([string]::IsNullOrWhiteSpace($TestsRoot)) {
    $TestsRoot = Join-Path $repoRoot 'tests'
}

if ([string]::IsNullOrWhiteSpace($ThresholdsFile)) {
    $ThresholdsFile = Join-Path $PSScriptRoot 'assembly-thresholds.json'
}

if (-not (Test-Path -LiteralPath $ThresholdsFile)) {
    throw "Thresholds file not found: $ThresholdsFile"
}

if (-not (Test-Path -LiteralPath $CoverageRoot)) {
    throw "Coverage directory not found: $CoverageRoot. Run tests with CollectCoverage=true first."
}

$thresholds = Get-Content -LiteralPath $ThresholdsFile -Raw | ConvertFrom-Json
$defaultMinimum = [double]$thresholds.defaultMinLineRate
$overrides = @{}

if ($thresholds.assemblies) {
    foreach ($property in $thresholds.assemblies.PSObject.Properties) {
        $overrides[$property.Name] = [double]$property.Value
    }
}

function Get-PrimaryAssemblyName {
    param([string] $TestProjectName)
    return $TestProjectName -replace '\.Tests$', ''
}

function Get-MinimumLineRate {
    param([string] $AssemblyName)

    if ($overrides.ContainsKey($AssemblyName)) {
        return $overrides[$AssemblyName]
    }

    return $defaultMinimum
}

function Get-LineRateFromCobertura {
    param(
        [string] $CoberturaPath,
        [string] $AssemblyName
    )

    [xml] $coverage = Get-Content -LiteralPath $CoberturaPath -Raw
    $package = @($coverage.coverage.packages.package) |
        Where-Object { $_.name -eq $AssemblyName } |
        Select-Object -First 1

    if (-not $package) {
        return $null
    }

    return [double]$package.'line-rate'
}

$failures = [System.Collections.Generic.List[string]]::new()
$checked = 0

$testProjects = Get-ChildItem -LiteralPath $TestsRoot -Directory |
    Where-Object { $_.Name -like 'MyNet.*.Tests' }

foreach ($testProject in $testProjects) {
    $assemblyName = Get-PrimaryAssemblyName -TestProjectName $testProject.Name
    $coverageDir = Join-Path $CoverageRoot $testProject.Name

    if (-not (Test-Path -LiteralPath $coverageDir)) {
        if ($FailOnMissingCoverage) {
            $failures.Add("Missing coverage output for '$($testProject.Name)' (expected '$coverageDir').")
        }
        else {
            Write-Host "SKIP $($testProject.Name): no coverage output directory." -ForegroundColor Yellow
        }

        continue
    }

    $coberturaFile = Get-ChildItem -LiteralPath $coverageDir -File -Filter 'coverage*.cobertura.xml' |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $coberturaFile) {
        if ($FailOnMissingCoverage) {
            $failures.Add("No Cobertura file found for '$($testProject.Name)' in '$coverageDir'.")
        }
        else {
            Write-Host "SKIP $($testProject.Name): no Cobertura file." -ForegroundColor Yellow
        }

        continue
    }

    $lineRate = Get-LineRateFromCobertura -CoberturaPath $coberturaFile.FullName -AssemblyName $assemblyName
    $minimum = Get-MinimumLineRate -AssemblyName $assemblyName
    $requiredPercent = [math]::Round($minimum * 100, 2)
    $checked++

    if ($null -eq $lineRate) {
        $failures.Add("${assemblyName}: no package entry in '$($coberturaFile.Name)'.")
        continue
    }

    $percent = [math]::Round($lineRate * 100, 2)

    if ($lineRate + 1e-9 -lt $minimum) {
        $failures.Add("${assemblyName}: $percent% line coverage is below required $requiredPercent%.")
    }
    else {
        Write-Host "OK ${assemblyName}: $percent% (required $requiredPercent%)"
    }
}

if ($checked -eq 0) {
    throw "No coverage results were evaluated under '$CoverageRoot'."
}

Write-Host ''
Write-Host "Checked $checked assembly(s)." -ForegroundColor Cyan

if ($failures.Count -gt 0) {
    Write-Host 'Coverage threshold check failed:' -ForegroundColor Red
    foreach ($failure in $failures) {
        Write-Host " - $failure" -ForegroundColor Red
    }

    exit 1
}

Write-Host 'All coverage thresholds satisfied.' -ForegroundColor Green
exit 0
