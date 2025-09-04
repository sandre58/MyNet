# PowerShell script for version management with Nerdbank.GitVersioning

param(
    [Parameter(Mandatory=$false)]
    [string]$Action = "info",
    
    [Parameter(Mandatory=$false)]
    [string]$Project = "",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = ""
)

function Show-Usage {
    Write-Host "Usage: .\manage-versions.ps1 [Action] [Options]" -ForegroundColor Green
    Write-Host ""
    Write-Host "Actions:" -ForegroundColor Yellow
    Write-Host "  info       - Display version information for all projects"
    Write-Host "  get        - Display the version of a specific project"
    Write-Host "  nbgv       - Display Nerdbank.GitVersioning information"
    Write-Host "  status     - Display Git status and calculated versions"
    Write-Host "  help       - Display this help"
    Write-Host ""
    Write-Host "Options:" -ForegroundColor Yellow
    Write-Host "  -Project   - Project name (for get action)"
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Cyan
    Write-Host "  .\manage-versions.ps1 info"
    Write-Host "  .\manage-versions.ps1 get -Project 'src/MyNet.Utilities'"
    Write-Host "  .\manage-versions.ps1 nbgv"
    Write-Host "  .\manage-versions.ps1 status"
}

function Get-ProjectVersion {
    param([string]$ProjectPath)
    
    if (Test-Path "$ProjectPath/version.json") {
        $versionJson = Get-Content "$ProjectPath/version.json" | ConvertFrom-Json
        return $versionJson.version
    }
    return "No version.json found"
}

function Show-AllVersions {
    Write-Host "=== MyNet Projects Version Information ===" -ForegroundColor Green
    Write-Host ""
    
    # Main projects
    $projects = @(
        "src/MyNet.Utilities",
        "src/MyNet.Observable", 
        "src/MyNet.UI",
        "src/MyNet.Humanizer",
        "src/MyNet.Http",
        "src/MyNet.AutoMapper.Extensions",
        "src/MyNet.CsvHelper.Extensions",
        "src/MyNet.Utilities.Generator.Extensions",
        "src/MyNet.Utilities.Geography.Extensions",
        "src/MyNet.Utilities.Localization.Extensions",
        "src/MyNet.Utilities.Logging.NLog",
        "src/MyNet.Utilities.Mail.MailKit"
    )
    
    foreach ($project in $projects) {
        if (Test-Path $project) {
            $version = Get-ProjectVersion $project
            Write-Host "$project : $version" -ForegroundColor Cyan
        }
    }
    
    Write-Host ""
    Write-Host "Project groups:" -ForegroundColor Yellow
    if (Test-Path "src/Wpf") {
        Write-Host "src/Wpf : $(Get-ProjectVersion 'src/Wpf')" -ForegroundColor Cyan
    }
    if (Test-Path "src/Avalonia") {
        Write-Host "src/Avalonia : $(Get-ProjectVersion 'src/Avalonia')" -ForegroundColor Cyan
    }
}

function Show-NBGVInfo {
    Write-Host "=== Nerdbank.GitVersioning Information ===" -ForegroundColor Green
    
    # Check if nbgv is installed
    try {
        $nbgvVersion = nbgv --version 2>$null
        Write-Host "nbgv version: $nbgvVersion" -ForegroundColor Gray
    } catch {
        Write-Warning "nbgv is not installed. Install it with: dotnet tool install -g nbgv"
        return
    }
    
    Write-Host ""
    
    try {
        # Global version information
        Write-Host "Version: $(nbgv get-version -v Version)" -ForegroundColor Cyan
        Write-Host "SimpleVersion: $(nbgv get-version -v SimpleVersion)" -ForegroundColor Cyan
        Write-Host "AssemblyVersion: $(nbgv get-version -v AssemblyVersion)" -ForegroundColor Cyan
        Write-Host "PublicRelease: $(nbgv get-version -v PublicRelease)" -ForegroundColor Cyan
        Write-Host "PrereleaseVersion: $(nbgv get-version -v PrereleaseVersion)" -ForegroundColor Cyan
        
        Write-Host ""
        Write-Host "Git Info:" -ForegroundColor Yellow
        Write-Host "CommitId: $(nbgv get-version -v GitCommitId)" -ForegroundColor Gray
        Write-Host "CommitIdShort: $(nbgv get-version -v GitCommitIdShort)" -ForegroundColor Gray
        Write-Host "VersionHeight: $(nbgv get-version -v VersionHeight)" -ForegroundColor Gray
        
    } catch {
        Write-Error "Error running nbgv: $_"
    }
}

function Show-GitStatus {
    Write-Host "=== Git status and versions ===" -ForegroundColor Green
    Write-Host ""
    
    # Basic Git information
    Write-Host "Current branch: $(git branch --show-current)" -ForegroundColor Cyan
    Write-Host "Last commit: $(git log -1 --pretty=format:'%h - %s (%cr)')" -ForegroundColor Gray
    
    # Latest tags
    $latestTags = git tag --sort=-version:refname | Select-Object -First 3
    if ($latestTags) {
        Write-Host ""
        Write-Host "Latest tags:" -ForegroundColor Yellow
        foreach ($tag in $latestTags) {
            Write-Host "  $tag" -ForegroundColor Gray
        }
    }
    
    # File status
    $status = git status --porcelain
    if ($status) {
        Write-Host ""
        Write-Host "Modified files:" -ForegroundColor Yellow
        git status --short
    } else {
        Write-Host ""
        Write-Host "No modified files" -ForegroundColor Green
    }
    
    Write-Host ""
    Show-NBGVInfo
}

# Main execution
switch ($Action.ToLower()) {
    "info" {
        Show-AllVersions
    }
    "get" {
        if ([string]::IsNullOrEmpty($Project)) {
            Write-Error "The -Project parameter is required for the 'get' action"
            Show-Usage
            exit 1
        }
        $version = Get-ProjectVersion $Project
        Write-Host "$Project : $version"
    }
    "nbgv" {
        Show-NBGVInfo
    }
    "status" {
        Show-GitStatus
    }
    "help" {
        Show-Usage
    }
    default {
        Write-Error "Unknown action: $Action"
        Show-Usage
        exit 1
    }
}
