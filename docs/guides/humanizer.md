# Humanizer

**Package:** [MyNet.Humanizer](../../src/MyNet.Humanizer/README.md)

Localized **human-readable** text for lists, time spans, enums, SmartEnums, addresses, and arbitrary values via a display-text pipeline.

Requires **MyNet.Globalization** (call `AddLocalization()` / `AddHumanizer()` in order).

## Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization.Extensions;
using MyNet.Humanizer;

var services = new ServiceCollection();
services.AddGlobalization();
services.AddLocalization();
services.AddHumanizer(options =>
{
  // options: TimeLocalizationOptions — relative time thresholds, etc.
});

var provider = services.BuildServiceProvider();
provider.UseDisplayText(); // wires TextHumanizer static facade
```

`AddHumanizer` registers culture-scoped:

| Service | Purpose |
|---------|---------|
| `IListFormatter` | "A, B, and C" style lists |
| `ITimeHumanizer` | Relative time, durations |
| `IOrdinalizer` | 1st, 2nd, … |
| `IAddressFormatter` | Multi-line addresses |
| `IDisplayTextService` | Unified display for enums, SmartEnums, primitives |

Translation resources: `DateTimeResources`, `ListResources`, etc. are registered automatically.

### Custom enum display

```csharp
services.AddDisplayTextStrategy<MyEnum, MyEnumDisplayTextStrategy>();
```

Implement `IDisplayTextStrategy<T>` for custom formatting rules.

---

## Lists

```csharp
using MyNet.Humanizer.Facade;
using MyNet.Humanizer.Formatting.Collections;

var text = new[] { "Apple", "Banana", "Cherry" }.Humanize();
// en: "Apple, Banana, and Cherry"

var orList = new[] { "A", "B", "C" }.Humanize(
    new ListFormattingOptions { Conjunction = ListConjunction.Or, UseOxfordComma = true });
```

Culture comes from current localization context when using extensions; inject `IListFormatter` for explicit culture:

```csharp
var formatter = localizationProviders.GetRequired<IListFormatter>(culture);
var text = formatter.Format(items);
```

---

## Time spans

```csharp
using MyNet.Humanizer.Facade;

var text = TimeSpan.FromSeconds(90).Humanize();
// "1 minute, 30 seconds" (culture-dependent)

var relative = TimeSpan.FromHours(2).Humanize(
    new TimeHumanizationOptions { Tense = Tense.Past });
```

Low-level API: inject `ITimeHumanizer` and call `HumanizeRelativeTime` / `HumanizeDuration`.

Pair with [MyNet.Temporal](foundations.md#mynettemporal) for decomposition before display.

---

## Enums and SmartEnums

```csharp
using MyNet.Humanizer.Facade;

var label = MyEnum.Active.Humanize();
var shortLabel = MyEnum.Active.Humanize(
    new DisplayTextOptions { Style = DisplayStyle.Short });

// SmartEnum (MyNet.Primitives)
var countryLabel = Country.France.Humanize();
```

`Dehumanize<TEnum>()` parses display text back to enum values when configured.

---

## Addresses

```csharp
using MyNet.Geography;
using MyNet.Humanizer.Formatting.Addresses;

// Via IAddressFormatter from localization providers
var address = new Address("10 Rue de Rivoli", "75001", "Paris", Country.France);
var formatted = addressFormatter.Format(address);
```

Culture-specific formatters: English, French, invariant (registered in `AddHumanizer`).

---

## Display text service

For types without a dedicated extension:

```csharp
using MyNet.Humanizer.Display;
using MyNet.Humanizer.Facade;

var text = TextHumanizer.Humanize(myValue, new DisplayTextOptions(), culture);
// or inject IDisplayTextService
```

Configure static facade once: `TextHumanizer.Configure(displayTextService)`.

---

## Integration with Globalization

Humanizer providers are resolved through `ILocalizationProviderResolver` / `ICultureScopedServiceSource<T>`. See [Globalization guide](globalization.md) for `ITranslationService` and resource keys.

Geography localized names: [Geography.Localization](../../src/MyNet.Geography.Localization/README.md) + Humanizer.

---

## Testing

- `tests/MyNet.Humanizer.Tests/Extensions/` — extension methods
- `tests/MyNet.Humanizer.Tests/Temporal/` — time humanizer
- Set culture explicitly in tests: `CultureInfo.GetCultureInfo("fr-FR")`

## Related

- [Globalization](globalization.md)
- [Geography](geography.md)
- [Foundations](foundations.md)
