#Requires -Version 5.1
param(
    [string] $CoverageRoot = '',
    [string] $ReportDirectory = '',
    [switch] $IncludeLegacyResults,
    [switch] $OpenReport
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

if ([string]::IsNullOrWhiteSpace($CoverageRoot)) {
    $CoverageRoot = Join-Path $repoRoot 'TestResults\coverage'
}

if ([string]::IsNullOrWhiteSpace($ReportDirectory)) {
    $ReportDirectory = Join-Path $repoRoot 'TestResults\coverage-report'
}

Push-Location $repoRoot
try {
    dotnet tool restore | Out-Null

    $reportPatterns = @(
        (Join-Path $CoverageRoot '**/coverage.cobertura.xml'),
        (Join-Path $CoverageRoot '**/coverage.*.cobertura.xml')
    )

    if ($IncludeLegacyResults) {
        $reportPatterns += @(
            (Join-Path $repoRoot 'TestResults/**/coverage.cobertura.xml'),
            (Join-Path $repoRoot 'TestResults/**/coverage.*.cobertura.xml')
        )
    }

    $reportPatterns = $reportPatterns | ForEach-Object { $_.Replace('\', '/') }

    $files = foreach ($pattern in $reportPatterns) {
        Get-ChildItem -Path $pattern -ErrorAction SilentlyContinue
    }

    $files = $files | Sort-Object FullName -Unique

    if (-not $files) {
        throw "No Cobertura files found under '$CoverageRoot'. Run tests with CollectCoverage=true first (see Run-SolutionCoverage.ps1)."
    }

    Write-Host "Merging $($files.Count) coverage file(s)..." -ForegroundColor Cyan

    if (Test-Path -LiteralPath $ReportDirectory) {
        Remove-Item -LiteralPath $ReportDirectory -Recurse -Force
    }

    New-Item -ItemType Directory -Path $ReportDirectory -Force | Out-Null

    $reportsArg = $reportPatterns -join ';'

    dotnet tool run reportgenerator `
        "-reports:$reportsArg" `
        "-targetdir:$ReportDirectory" `
        "-reporttypes:Html;HtmlSummary;TextSummary" `
        "-verbosity:Warning"

    if ($LASTEXITCODE -ne 0) {
        throw "ReportGenerator failed with exit code $LASTEXITCODE."
    }

    $summaryPath = Join-Path $ReportDirectory 'Summary.txt'
    if (Test-Path -LiteralPath $summaryPath) {
        Write-Host ''
        Get-Content -LiteralPath $summaryPath | Write-Host
    }

    Write-Host ''
    Write-Host "Coverage report: $ReportDirectory\index.html" -ForegroundColor Green

    if ($OpenReport) {
        Start-Process (Join-Path $ReportDirectory 'index.html')
    }
}
finally {
    Pop-Location
}
