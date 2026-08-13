<div align="center">

# MyNet.Temporal

<img src="../../assets/MyNetTemporal.png" alt="MyNet.Temporal" width="96" height="96" />

*Public temporal library: civil and absolute intervals, calendar amounts, fluent date helpers, and TimeSpan decomposition.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Temporal)](https://www.nuget.org/packages/MyNet.Temporal)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Intervals** | `DateTimePeriod` / `DateTimeOffsetPeriod` (half-open), configurable ranges, `DateRange`, `TimeRange` |
| **CalendarAmount** | Years/months + fixed `TimeSpan` for calendar arithmetic (not a universal duration) |
| **Fluent helpers** | Begin/end of day/week/month, builders, Offset parity |
| **Decomposition** | `TimeSpan` breakdown into readable parts |
| **JSON** | System.Text.Json converters for Periods + `CalendarAmount` |

---

## Mental model

| Layer | BCL citizen | MyNet types |
| :---- | :---------- | :---------- |
| Civil / UI | `DateTime` (Kind explicit) | `DateTimePeriod`, `DateTimeRange` |
| Absolute | `DateTimeOffset` | `DateTimeOffsetPeriod`, `DateTimeOffsetRange` |
| Calendar day / clock | `DateOnly` / `TimeOnly` | `DateRange` (closed), `TimeRange` (closed, overnight) |
| Fixed duration | `TimeSpan` | — |
| Calendar amount | — | `CalendarAmount` (`AddYears`/`AddMonths`) |

Periods of instants are **`[start, end)`**. Prefer Offset types for APIs and multi-zone domain code.

---

## Installation

```bash
dotnet add package MyNet.Temporal
```

## Quick start

```csharp
using MyNet.Temporal;
using MyNet.Temporal.Intervals;
using MyNet.Temporal.Serialization;
using System.Text.Json;

var occupation = new DateTimeOffsetPeriod(
    DateTimeOffset.Parse("2026-06-15T09:00:00+02:00"),
    DateTimeOffset.Parse("2026-06-15T10:30:00+02:00"));

var later = DateTime.Today.AddCalendarAmount(2.Months());

var options = new JsonSerializerOptions().AddMyNetTemporalConverters();
var json = JsonSerializer.Serialize(occupation, options);
```

```csharp
using MyNet.Temporal.Decomposition.Extensions;

var parts = TimeSpan.FromHours(25).Decompose();
```

---

## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives) — generic `Interval<T>` / `NumericRange`
- [MyNet.Humanizer](https://www.nuget.org/packages/MyNet.Humanizer) — localized duration display

---

## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
