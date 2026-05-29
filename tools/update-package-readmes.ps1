# Updates src/*/README.md with icon, badges, features, and copyright.
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$year = (Get-Date).Year
$copyright = ('Copyright {0} 2016-{1} - St{2}phane ANDRE. All Rights Reserved.' -f [char]0x00A9, $year, [char]0x00E9)

$packages = @(
    @{
        Id       = 'MyNet.Primitives'
        Icon     = 'MyNetPrimitives.png'
        Tagline  = 'Foundation library for the MyNet suite: SmartEnum, intervals, unit conversions, comparers, sequences, and shared primitives.'
        Features = @(
            'SmartEnum|Strongly typed name/value pairs'
            'Intervals|Ranges, sequences, and numeric bounds'
            'Conversions|Unit conversions and physical quantities'
            'Comparers|Guards, comparers, and minimal BCL-only dependencies'
        )
    },
    @{
        Id       = 'MyNet.Text'
        Icon     = 'MyNetText.png'
        Tagline  = 'String processing: templating, slugification, sanitization, normalization, truncation, redaction, and formatting pipelines.'
        Features = @(
            'Templates|Token-based templates with culture-aware formatting'
            'Transforms|Slugify, sanitize, normalize, truncate, and redact'
            'Pipelines|Composable text transformation pipelines'
            'DI extensions|Random generator registration for tests'
        )
    },
    @{
        Id       = 'MyNet.Utilities'
        Icon     = 'MyNetUtilities.png'
        Tagline  = 'Cross-cutting utilities: caching, encryption, authentication helpers, progress reporting, threading, deferral, and DI-friendly services.'
        Features = @(
            'Caching|In-memory storage with absolute and sliding expiration'
            'Security|AES encryption and authentication helpers'
            'Progress|Reporting, debouncing, and serialized async runners'
            'Threading|Deferral, suspending scopes, and coordination'
        )
    },
    @{
        Id       = 'MyNet.Generator'
        Icon     = 'MyNetGenerator.png'
        Tagline  = 'Pseudo-random value generators, weighted sampling, and sequence helpers for unit tests, benchmarks, and prototyping.'
        Features = @(
            'IRandomGenerator|Facade for ints, strings, and collections'
            'Sampling|Weighted picks and repeatable test doubles'
            'Culture-aware|Random data building blocks per locale'
            'Lightweight|No host framework assumptions'
        )
    },
    @{
        Id       = 'MyNet.Reflection'
        Icon     = 'MyNetReflection.png'
        Tagline  = 'Cached reflection accessors, expression helpers, and object-graph utilities to reduce runtime reflection overhead.'
        Features = @(
            'Accessors|Cached property and member accessors'
            'Expressions|Expression-tree helpers for fast get/set'
            'Object graphs|Traversal and inspection utilities'
            'Infrastructure|Focused API for higher-level MyNet packages'
        )
    },
    @{
        Id       = 'MyNet.Metadata'
        Icon     = 'MyNetMetadata.png'
        Tagline  = 'Central metadata registry for types and members with pluggable feature providers for reflection-light, extensible APIs.'
        Features = @(
            'Registry|Central type and member metadata store'
            'Providers|Pluggable metadata feature providers'
            'Lookups|Reflection-light resolution for UI and generators'
            'Observable|Foundation for metadata source generation'
        )
    },
    @{
        Id       = 'MyNet.Temporal'
        Icon     = 'MyNetTemporal.png'
        Tagline  = 'TimeSpan decomposition, duration breakdown, and temporal helpers built on MyNet.Primitives for readable time representations.'
        Features = @(
            'Decomposition|TimeSpan breakdown into readable parts'
            'Durations|Helpers for UI labels and logging'
            'Primitives|Built on MyNet.Primitives temporal types'
            'Humanizer|Pairs with localized display formatters'
        )
    },
    @{
        Id       = 'MyNet.Collections'
        Icon     = 'MyNetCollections.png'
        Tagline  = 'Observable collections with range notifications, keyed collections, synchronizers, and batch update helpers for MVVM.'
        Features = @(
            'Range collection|Batch AddRange and ReplaceRange with notifications'
            'Keyed patterns|Selectable and keyed collection helpers'
            'Synchronizers|Multi-source collection sync for MVVM'
            'Notifications|Efficient range change events for UI binding'
        )
    },
    @{
        Id       = 'MyNet.Messaging'
        Icon     = 'MyNetMessaging.png'
        Tagline  = 'Weak-reference in-process messenger for loosely coupled communication between view models and services (MVVM-friendly).'
        Features = @(
            'Weak references|Messenger that avoids view model leaks'
            'Messaging|Register, send, and unregister patterns'
            'Decoupling|Loose communication without a service locator'
            'Standalone|No required MyNet package dependencies'
        )
    },
    @{
        Id       = 'MyNet.Observable'
        Icon     = 'MyNetObservable.png'
        Tagline  = 'MVVM-oriented observable models with INotifyPropertyChanged, edition tracking, FluentValidation, metadata, and an included Roslyn source generator.'
        Features = @(
            'ObservableObject|INotifyPropertyChanged base with change notification'
            'Behaviors|Edition tracking and FluentValidation integration'
            'Source generator|Roslyn codegen for observable properties'
            'Metadata|Labels, groups, and UI hints for bound models'
        )
    },
    @{
        Id       = 'MyNet.UI'
        Icon     = 'MyNetUI.png'
        Tagline  = 'UI-framework-agnostic presentation layer: view models, shell, dialogs, navigation, notifications, and toasts. You provide concrete UI (WPF, Avalonia, etc.).'
        Features = @(
            'Navigation|View locators and navigation services'
            'Dialogs|Presenter abstraction and shell host patterns'
            'Notifications|Toast and notification managers for MVVM'
            'Theming|Contracts implemented by your UI host (WPF, Avalonia, etc.)'
        )
    },
    @{
        Id       = 'MyNet.Globalization'
        Icon     = 'MyNetGlobalization.png'
        Tagline  = 'Culture-aware formatting, localization services, inflection, and translation resource registration for DI-based applications.'
        Features = @(
            'Formatting|Culture-aware formatting and localization'
            'Translations|Resource registration for DI hosts'
            'Inflection|Grammatically correct UI text helpers'
            'DI|Microsoft.Extensions.DependencyInjection integration'
        )
    },
    @{
        Id       = 'MyNet.Humanizer'
        Icon     = 'MyNetHumanizer.png'
        Tagline  = 'Localized human-readable strings for dates, enums, lists, geography, and common .NET types (display beyond raw ToString()).'
        Features = @(
            'Formatters|Localized dates, enums, and lists'
            'Domains|Geography and temporal display helpers'
            'UI binding|Culture-aware human-readable output'
            'Stack|Built on Globalization and Temporal packages'
        )
    },
    @{
        Id       = 'MyNet.Geography'
        Icon     = 'MyNetGeography.png'
        Tagline  = 'ISO 3166 countries and continents, postal addresses, coordinates, and a pluggable country-flag provider contract.'
        Features = @(
            'ISO 3166|Country and continent model'
            'Addresses|Postal addresses and coordinates'
            'Flags|Pluggable IFlagProvider contract'
            'Core types|Shared geography model across the suite'
        )
    },
    @{
        Id       = 'MyNet.Geography.Resources'
        Icon     = 'MyNetGeography.png'
        Tagline  = 'Embedded multi-resolution country flag PNG assets and a default IFlagProvider for MyNet.Geography.'
        Features = @(
            'Flag assets|Embedded multi-resolution country PNGs'
            'IFlagProvider|Default implementation for Geography'
            'DI|Registration via AddGeographyFlags()'
            'Satellite|Optional resources for Geography consumers'
        )
    },
    @{
        Id       = 'MyNet.Geography.Localization'
        Icon     = 'MyNetGeography.png'
        Tagline  = 'Localized country display names and MyNet.Humanizer formatters for geography types in MyNet.Geography.'
        Features = @(
            'Display names|Localized country and region labels'
            'Resources|Satellite .resx files for multiple cultures'
            'Humanizer|Formatters for geography types'
            'Extension|Culture-aware labels for MyNet.Geography'
        )
    },
    @{
        Id       = 'MyNet.Google'
        Icon     = 'MyNetGoogle.png'
        Tagline  = 'Google Maps integration: geocoding, directions, distance matrix queries, and map or Street View URL builders.'
        Features = @(
            'Geocoding|Forward and reverse geocoding helpers'
            'Routing|Directions and distance matrix queries'
            'URLs|Map and Street View link builders'
            'Geography|Integrates with coordinates and addresses'
        )
    },
    @{
        Id       = 'MyNet.IO'
        Icon     = 'MyNetIO.png'
        Tagline  = 'File and path helpers, auto-save coordinators, and portable registry abstractions for desktop and cross-platform apps.'
        Features = @(
            'Files|Path and file extension helpers'
            'Auto-save|Coordinators for documents and settings'
            'Registry|Portable abstractions for desktop apps'
            'UI integration|Workspace and recent-file scenarios'
        )
    },
    @{
        Id       = 'MyNet.Platform.Windows'
        Icon     = 'MyNetPlatformWindows.png'
        Tagline  = 'Windows platform adapters: registry access, authentication integrations, and MAPI mail support for MyNet.IO and MyNet.Utilities.'
        Features = @(
            'Registry|Windows implementations for MyNet.IO'
            'Authentication|Windows identity integrations'
            'MAPI|Desktop mail support via native APIs'
            'Adapters|Platform code kept out of portable packages'
        )
    },
    @{
        Id       = 'MyNet.Http'
        Icon     = 'MyNetHttp.png'
        Tagline  = 'HTTP client helpers and Web API consumption utilities on HttpClient and System.Text.Json.'
        Features = @(
            'WebApiService|Typed REST calls with System.Text.Json'
            'Timeouts|Per-request timeout configuration'
            'Requests|HttpClient-friendly building patterns'
            'Responses|Helpers for Web API consumption'
        )
    },
    @{
        Id       = 'MyNet.Mail'
        Icon     = 'MyNetMail.png'
        Tagline  = 'Email message and attachment models, address parsing, and SMTP service abstractions independent of a mail transport.'
        Features = @(
            'Email builder|Fluent API for recipients, subject, and body'
            'Attachments|Models and address parsing'
            'IMailService|Transport-independent mail abstraction'
            'MailKit|Optional SMTP delivery via sibling package'
        )
    },
    @{
        Id       = 'MyNet.Mail.MailKit'
        Icon     = 'MyNetMail.png'
        Tagline  = 'MailKit implementation of MyNet.Mail IMailService with TLS configuration, connection reuse, and Microsoft.Extensions.DependencyInjection registration.'
        Features = @(
            'MailKit|IMailService implementation for SMTP'
            'TLS|Configuration and connection reuse'
            'DI|AddMailKit() registration extensions'
            'Production|Ready transport for MyNet.Mail models'
        )
    },
    @{
        Id       = 'MyNet.Fakers'
        Icon     = 'MyNetFakers.png'
        Tagline  = 'Locale-aware fake data for tests and demos: names, addresses, emails, URLs, and related patterns.'
        Features = @(
            'Generators|Culture-aware person, address, and internet data'
            'Stack|Built on Generator and Geography packages'
            'Fixtures|Realistic data for MVVM and API demos'
            'Cultures|Integrates with Globalization settings'
        )
    }
)

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
    if ($body -match '## Related packages') {
        $body = $body -replace '(\r?\n)(## Related packages)', "`$1---`$1`$2"
    }
    if ($body -match '## Documentation') {
        $body = $body -replace '(\r?\n)(## Documentation)', "`$1---`$1`$2"
    }
    return $body.Trim()
}

function New-PackageHeader {
    param($Id, $Tagline, $Icon, $Features)
    $summary = Get-Summary $Tagline
    $featureRows = ($Features | ForEach-Object { Format-FeatureRow $_ }) -join "`n"
    @"
<div align="center">

# $Id

<img src="../../assets/$Icon" alt="$Id" width="96" height="96" />

*$summary*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
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
    $idx = $raw.IndexOf('## Installation')
    if ($idx -lt 0) { return $null }
    $body = $raw.Substring($idx).Trim()
    $body = $body -replace '(?s)\r?\n## License\r?\n.*$', ''
    $body = $body -replace '(?s)\r?\n## Copyright\r?\n.*$', ''
    $body = $body -replace '(?s)\r?\n---\r?\n\r?\n<div align="center">.*$', ''
    $body = $body -replace '^(?:---\s*)+', ''
    return $body.Trim()
}

function New-PackageFooter {
    param([string]$CopyrightText)
    @"

---

<div align="center">

<sub>

$CopyrightText

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
"@
}

foreach ($pkg in $packages) {
    $readmePath = Join-Path $repoRoot "src\$($pkg.Id)\README.md"
    if (-not (Test-Path $readmePath)) {
        Write-Warning "Missing README: $readmePath"
        continue
    }

    $body = Get-PackageBody -ReadmePath $readmePath
    if (-not $body) {
        Write-Warning "No ## Installation in $readmePath"
        continue
    }
    $body = Format-PackageBody $body

    $header = New-PackageHeader @pkg
    $footer = New-PackageFooter $copyright
    $newContent = $header.TrimEnd() + "`n`n---`n`n" + $body + $footer + "`n"
    $newContent = [regex]::Replace($newContent, '(\r?\n)(?:---\s*){2,}(?=\r?\n)', "`$1---`$1")
    if (-not $newContent.EndsWith("`n")) { $newContent += "`n" }

    $utf8 = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($readmePath, $newContent, $utf8)
    Write-Host "Updated $($pkg.Id)"
}

Write-Host "Done."
