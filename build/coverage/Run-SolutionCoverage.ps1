#Requires -Version 5.1
<#
.SYNOPSIS
    Runs all solution tests with Coverlet, verifies thresholds, and produces a merged HTML report.

.EXAMPLE
    .\build\coverage\Run-SolutionCoverage.ps1

.EXAMPLE
    .\build\coverage\Run-SolutionCoverage.ps1 -Configuration Release -OpenReport
#>
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug',

    [string] $Solution = 'MyNet.slnx',

    [switch] $SkipThresholdCheck,

    [switch] $SkipReport,

    [switch] $IncludeLegacyResults,

    [switch] $OpenReport
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$solutionPath = Join-Path $repoRoot $Solution
$coverageRoot = Join-Path $repoRoot 'TestResults\coverage'
$reportDir = Join-Path $repoRoot 'TestResults\coverage-report'

if (-not (Test-Path -LiteralPath $solutionPath)) {
    throw "Solution not found: $solutionPath"
}

Push-Location $repoRoot
try {
    Write-Host "Running tests with coverage ($Configuration)..." -ForegroundColor Cyan

    dotnet test $solutionPath `
        --configuration $Configuration `
        --verbosity minimal `
        /p:CollectCoverage=true

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet test failed with exit code $LASTEXITCODE."
    }

    if (-not $SkipThresholdCheck) {
        & (Join-Path $PSScriptRoot 'Verify-CriticalCoverage.ps1') `
            -CoverageRoot $coverageRoot `
            -FailOnMissingCoverage
    }

    if ($SkipReport) {
        Write-Host "Skipping merged report generation (-SkipReport)." -ForegroundColor Yellow
        return
    }

    & (Join-Path $PSScriptRoot 'Merge-SolutionCoverageReports.ps1') `
        -CoverageRoot $coverageRoot `
        -ReportDirectory $reportDir `
        -IncludeLegacyResults:$IncludeLegacyResults `
        -OpenReport:$OpenReport
}
finally {
    Pop-Location
}
