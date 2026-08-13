# Foundations

Core building blocks used across the MyNet suite. Prefer the **smallest package** that exposes the API you need.

## Package map

| Package | Responsibility | README |
|---------|----------------|--------|
| **MyNet.Primitives** | `SmartEnum`, generic intervals, conversions, comparers, sequences | [README](../../src/MyNet.Primitives/README.md) |
| **MyNet.Text** | Templating, slugify, sanitize, normalize, truncate, redact | [README](../../src/MyNet.Text/README.md) |
| **MyNet.Utilities** | Cache, encryption, progress, threading, deferral, logging helpers | [README](../../src/MyNet.Utilities/README.md) |
| **MyNet.Generator** | `IRandomGenerator`, sampling, random strings | [README](../../src/MyNet.Generator/README.md) |
| **MyNet.Reflection** | Cached property accessors, expression helpers | [README](../../src/MyNet.Reflection/README.md) |
| **MyNet.Metadata** | Type/member metadata registry + features | [README](../../src/MyNet.Metadata/README.md) |
| **MyNet.Temporal** | Date/time intervals, `CalendarAmount`, fluent helpers, `TimeSpan` decomposition | [README](../../src/MyNet.Temporal/README.md) |

---

## MyNet.Primitives

### SmartEnum

```csharp
using MyNet.Primitives;

public sealed class Status : SmartEnum<Status, string>
{
    public static readonly Status Draft = new(nameof(Draft), "draft");
    public static readonly Status Published = new(nameof(Published), "published");

    private Status(string name, string value) : base(name, value) { }
}
```

### Intervals and ranges

```csharp
using MyNet.Primitives.Intervals;

var range = new NumericRange<int>(1, 100);
var contains = range.Contains(42);
```

### Unit conversion

```csharp
using MyNet.Primitives.Conversion;

var celsius = TemperatureConverter.Instance.Convert(32, TemperatureUnit.Fahrenheit, TemperatureUnit.Celsius);
```

---

## MyNet.Text

### Slugify

```csharp
using MyNet.Text.Extensions;

var slug = "Hello World!".Slugify();
```

### Templates

```csharp
using System.Globalization;
using MyNet.Text.Templating;

var options = new TextTemplateOptionsBuilder()
    .WithArgument("Name", "Ada")
    .Build();

var text = new TemplateTransform(options).Apply("Hello, {Name}!", CultureInfo.InvariantCulture);
```

### Pipelines

Combine transforms via `TextPipeline` (normalization, sanitization, slugification). See `tests/MyNet.Text.Tests`.

---

## MyNet.Utilities

No `AddUtilities()` in this assembly — register types in your host or use static helpers.

### Cache

```csharp
using MyNet.Utilities.Caching;

var cache = new CacheStorage<string, string>();
var value = cache.GetFromCacheOrFetch("user:1", () => LoadUserName(1));
```

Policies: `AbsoluteExpirationPolicy`, `SlidingExpirationPolicy` under `MyNet.Utilities.Caching.Policies`.

### Encryption

```csharp
using System.Text;
using MyNet.Utilities.Encryption;

var key = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // 16 bytes for AES-128
var crypto = new AesEncryptionService(key);
var cipher = crypto.Encrypt("secret");
var plain = crypto.Decrypt(cipher);
```

### Progress

```csharp
using MyNet.Utilities.Progress;

using var progresser = new Progresser(totalSteps: 100);
progresser.Report(50, "Half done");
```

### Threading helpers

- `Debouncer` — coalesce rapid calls
- `SingleTaskRunner` / `SingleTaskDeferrer` — serialize async work
- `Suspender` — nested suspend scopes

See `tests/MyNet.Utilities.Tests`.

---

## MyNet.Generator

```csharp
using MyNet.Generator;
using MyNet.Generator.Facade;

var n = RandomGenerator.Current.Int(1, 10);
var pick = RandomGenerator.Current.Item(["a", "b", "c"]);
var label = RandomGenerator.Current.String(8);
```

Replace `RandomGenerator.Current` in tests via `RandomGenerator.Current = myGenerator`.

DI (from Fakers package): `services.AddRandomGenerator()` in `MyNet.Text` extensions.

---

## MyNet.Reflection

Use for fast get/set without repeating `PropertyInfo` lookups. Patterns in `tests/MyNet.Reflection.Tests`.

Typical use: infrastructure libraries (Humanizer, Observable metadata) rather than app code.

---

## MyNet.Metadata

Registry for **features** attached to types and properties. Most app developers use the **Observable metadata generator** instead of manual setup:

See [Observable guide → Metadata generation](observable.md#metadata-generation).

Manual fluent API (tests, edge cases):

```csharp
MetadataRegistry.For<MyViewModel>()
    .Property(x => x.Title)
    .UpdateOnCultureChanged();
```

---

## MyNet.Temporal

Public temporal surface. Depends on `MyNet.Primitives` for generic `Interval<T>`.

### Contracts

| Layer | Citizen | Types | Notes |
|-------|---------|-------|-------|
| Civil / UI | `DateTime` | `DateTimePeriod`, `DateTimeRange` | Kind not normalized; Unspecified = consumer civil time |
| Absolute | `DateTimeOffset` | `DateTimeOffsetPeriod`, `DateTimeOffsetRange` | Prefer for APIs / multi-offset |
| Day / clock | `DateOnly` / `TimeOnly` | `DateRange` (closed), `TimeRange` (closed, overnight) | |
| Fixed duration | `TimeSpan` | — | Occupation length |
| Calendar amount | — | `CalendarAmount` | `AddYears`/`AddMonths`; use `ToApproximateTimeSpan()` only when approximating |

Instant periods are half-open **`[start, end)`**.

```csharp
using MyNet.Temporal;
using MyNet.Temporal.Intervals;
using MyNet.Temporal.Serialization;
using System.Text.Json;

var period = new DateTimeOffsetPeriod(
    DateTimeOffset.Parse("2026-06-15T09:00:00+02:00"),
    DateTimeOffset.Parse("2026-06-15T10:00:00+02:00"));

var options = new JsonSerializerOptions().AddMyNetTemporalConverters();
```

### Decomposition

```csharp
using MyNet.Temporal.Decomposition.Extensions;

var parts = TimeSpan.FromHours(25).Decompose();
// IReadOnlyList<TimeUnitValue> — use with Humanizer for display
```

Presets: `.Humanized()`, `.Compact()`, `.Full()` extension methods on `TimeSpan`.

---

## When to reference which package

| Need | Package |
|------|---------|
| Rich enum, generic intervals, units | Primitives only |
| Dates, periods, calendar amounts, TimeSpan decompose | Temporal (+ Primitives) |
| String pipelines | Text (+ Primitives, Generator) |
| App-wide cache/crypto/progress | Utilities |
| Test random data | Generator or [Fakers](fakers.md) |
| Fast reflection | Reflection |
| MVVM metadata / behaviors | [Observable](observable.md) (includes Metadata transitively) |

## Related

- [Observable models](observable.md)
- [Fakers](fakers.md)
- [Globalization](globalization.md)
