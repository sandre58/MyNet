# Globalization & localization

**Package:** [MyNet.Globalization](../../src/MyNet.Globalization/README.md)

Culture management, localization services, inflection, and translation resource registration for DI-based applications.

## DI registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization.Extensions;

var services = new ServiceCollection();
services.AddGlobalization();
services.AddLocalization();
services.AddInflection();
```

Configure options via `AddGlobalization(options => { ... })` and register `.resx` sources with `AddTranslationResource`.

### Integration with Observable

Observable behaviors can react to culture changes:

```csharp
this.ReactOnCultureChanged(cultureService);
```

See [Observable models](observable.md).

---

## Translation system

The MyNet translation system is a composable localization framework that separates pure translation logic from contextual concerns. It supports multiple languages, display styles, cultures, and includes inflection fallback and dynamic resource registration.

### Architecture overview

#### 1. Pure translator (`ITranslator`)

**Responsibility**: Stateless, culture-explicit translation  
**Key characteristic**: No side effects, fully testable without setup

```csharp
public interface ITranslator
{
    string Translate(string key, TranslationOptions options, CultureInfo culture, string? resourceKey = null);
}
```

**Usage**:

```csharp
var result = translator.Translate("greeting", options, new CultureInfo("en-US"));
```

**When to use**:

- Server-side where culture is explicit
- Testing translations for specific cultures
- Sharing translation logic across different contexts

#### 2. Contextual service (`ITranslationService`)

**Responsibility**: Wraps translator with culture context  
**Key characteristic**: Culture comes from environment, ergonomic API

```csharp
public interface ITranslationService
{
    string Translate(string key, TranslationOptions options);
}
```

**Usage**:

```csharp
// Culture is implicit from context
var result = service.Translate("greeting", options);
```

**When to use**:

- UI where current culture is contextual
- Reducing parameter passing
- Simplifying API surface

### Key concepts

#### Display style

Controls the form of translation output:

- **Default**: Full natural language form
- **Short**: Abbreviated but recognizable
- **Abbreviation**: Technical abbreviation (e.g., USD for currency)
- **Symbol**: Graphical representation (e.g., $ for currency)
- **Narrow**: Ultra-compact (e.g., single character)

```csharp
var builder = new TranslationOptionsBuilder()
    .WithStyle(DisplayStyle.Symbol)
    .Build();
```

#### Culture fallback chain

Automatic resolution order when a translation is not found is governed by `ICultureFallbackPolicy`:

- **`CultureFallbackPolicies.ParentCulture`** (default): en-US → en → invariant
- **`CultureFallbackPolicies.None`**: no fallback (exact match only)
- **Custom policy**: implement `ICultureFallbackPolicy.GetFallback(CultureInfo)`

Configure via `GlobalizationOptions.CultureFallbackPolicy` (DI) or directly on `LocalizationProviderFactoryBuilder.FallbackPolicy`.

#### Inflection fallback

Applies grammar rules when direct translation is unavailable:

- **Flexible policy**: Default/Short/Narrow styles — inflection allowed
- **Strict policy**: Symbol/Abbreviation — no inflection

Example:

```csharp
// English: "person" -> "people" (inflection)
var options = new TranslationOptionsBuilder()
    .WithQuantity(2)
    .Build();
var result = translator.Translate("person", options, en_US);
// Result: "people"
```

### Working with TranslationOptions

#### Simple translation

```csharp
var options = TranslationOptionsPresets.Default;
var text = service.Translate("greeting", options);
```

#### With quantity (pluralization)

```csharp
var options = new TranslationOptionsBuilder()
    .WithQuantity(5)
    .Build();

var text = service.Translate("items", options);
```

#### With display style

```csharp
var options = new TranslationOptionsBuilder()
    .WithStyle(DisplayStyle.Abbreviation)
    .Build();

var text = service.Translate("currency", options);
```

#### With named arguments

```csharp
var options = new TranslationOptionsBuilder()
    .WithArgument("name", "Alice")
    .WithArgument("count", 42)
    .Build();

var text = service.Translate("message", options);
// Template: "{name} has {count} items" → "Alice has 42 items"
```

#### Complex scenario

```csharp
var options = new TranslationOptionsBuilder()
    .WithStyle(DisplayStyle.Short)
    .WithQuantity(1000)
    .WithArgument("currency", "USD")
    .WithArgument("location", "Store A")
    .WithoutInflectionFallback()
    .Build();

var text = service.Translate("price_summary", options);
```

### Template rendering

Templates use `{placeholder}` or `{placeholder:format}` syntax:

```csharp
// Resource: "You have {quantity} items worth {amount:C2}"

var options = new TranslationOptionsBuilder()
    .WithQuantity(5)
    .WithArgument("amount", 99.99m)
    .Build();

var result = service.Translate("order_summary", options);
// en-US: "You have 5 items worth $99.99"
// fr-FR: "You have 5 items worth 99,99 €"
```

#### Built-in arguments

- **quantity**: Added automatically when `WithQuantity()` is called
- **Custom**: Any arguments via `WithArgument()` or `WithArguments()`

### Translation resource structure

Resources are organized by module:

```
Resources/
├── UIResources.resx          // UI strings (buttons, labels)
├── MessageResources.resx     // Messages and notifications
├── DateTimeResources.resx    // Temporal humanization
└── ListResources.resx        // List formatting
```

Register resources:

```csharp
var catalog = LocalizationContext.Catalog;
catalog.Register(nameof(UIResources), UIResources.ResourceManager);
catalog.Register(nameof(MessageResources), MessageResources.ResourceManager);
```

> In DI scenarios prefer `services.AddTranslationResource(nameof(UIResources), UIResources.ResourceManager)` which contributes resources during container build-up.

### Humanization integration

#### ListFormatter — lists with conjunctions

```csharp
var formatter = LocalizationContext.Providers.Get<IListFormatter>(culture);
// or, throwing if not found:
var formatter = LocalizationContext.Providers.GetRequired<IListFormatter>(culture);
// or, culture-scoped context:
var ctx = LocalizationContext.Providers.ForCulture(culture);
var formatter = ctx.GetRequired<IListFormatter>();

var result1 = formatter.Format(new[] { "apple", "banana", "cherry" });

var result2 = formatter.Format(
    new[] { "apple", "banana", "cherry" },
    new ListFormattingOptions
    {
        Conjunction = ListConjunction.Or,
        UseOxfordComma = true
    });
// "apple, banana, or cherry"
```

See [Humanizer](humanizer.md) and `tests/MyNet.Humanizer.Tests` for list formatting examples.

#### TimeHumanizer — relative time

```csharp
var humanizer = LocalizationContext.Providers.GetRequired<ITimeHumanizer>(culture);

var result = humanizer.HumanizeRelativeTime(2, TimeUnit.Hour, Tense.Past);
// en-US: "2 hours ago"
// fr-FR: "il y a 2 heures"
```

#### DisplayNameProvider — enums and SmartEnums

```csharp
var provider = LocalizationContext.Providers.Get<IDisplayNameProvider<MyEnum>>(culture);
var displayName = provider.GetDisplayName(MyEnum.Active);

var shortForm = provider.GetDisplayName(
    MyEnum.Active,
    new DisplayNameOptions { Style = DisplayStyle.Abbreviation });
```

### Display styles in practice

#### Currency example

| Key | Default | Short | Abbreviation | Symbol | Narrow |
|-----|---------|-------|--------------|--------|--------|
| currency | Dollar | USD | USD | $ | $ |

#### Status example

| Key | Default | Short | Abbreviation | Symbol | Narrow |
|-----|---------|-------|--------------|--------|--------|
| status_active | Active | Act | ACT | ✓ | ✓ |
| status_inactive | Inactive | Inact | INACT | ✗ | ✗ |

### Pluralization rules

Supported via language-specific inflectors:

| Language | Rule |
|----------|------|
| English | 0 → "zero"; 1 → "one"; 2+ → plural |
| French | 0,1 → singular; 2+ → plural |
| German | 0,1 → singular; 2+ → plural |

### Best practices

1. **Use `ITranslationService` in UI** — culture is implicit.
2. **Use `ITranslator` in core logic** — explicit and testable.
3. **Cache built `TranslationOptions`** — avoid rebuilding identical configuration.
4. **Use resource-specific lookups** — `Translate(..., nameof(UIResources))` for faster lookup.
5. **Leverage inflection for grammar** — `WithQuantity` + flexible display styles.
6. **Culture-aware formatting** — templates like `{amount:C2}` respect locale.
7. **Handle missing translations** — `UseKeyFallback()` or explicit checks.

### Advanced scenarios

#### Conditional display styles

```csharp
var style = userPreference switch
{
    UserUiMode.Minimal => DisplayStyle.Symbol,
    UserUiMode.Compact => DisplayStyle.Abbreviation,
    UserUiMode.Normal => DisplayStyle.Default,
    UserUiMode.Verbose => DisplayStyle.Short,
    _ => DisplayStyle.Default
};

var options = new TranslationOptionsBuilder()
    .WithStyle(style)
    .Build();
```

#### Dynamic resource registration

```csharp
var catalog = LocalizationContext.Catalog;
catalog.Register("PluginXResources", PluginXResources.ResourceManager);

var text = translator.Translate("plugin_greeting", options, culture, "PluginXResources");
```

#### Culture-aware list formatting

```csharp
var formatter = LocalizationContext.Localizer.GetProvider<IListFormatter>(culture);

var items = new[] { "apple", "banana", "cherry" };
var text = formatter.Format(items, new ListFormattingOptions
{
    Conjunction = ListConjunction.And,
    UseOxfordComma = culture.Name.StartsWith("en")
});
```

### Troubleshooting

#### Translation returns the key

Possible causes: key not found, culture not registered, resource module not registered.

```csharp
var provider = LocalizationContext.Providers.Get<IInflector>(requestedCulture);
var hasResource = catalog.Resources.ContainsKey(resourceKey);
var report = LocalizationContext.Diagnostics?.GetDiagnosticReport(); // DI only
```

#### Quantity not pluralizing

Ensure `UseInflectionFallback()` is enabled, style is not Strict (Symbol/Abbreviation), and the language inflector is configured.

#### Culture fallback not working

Check `CultureFallbackPolicy` — default is `ParentCulture`. To disable: `opt.CultureFallbackPolicy = CultureFallbackPolicies.None`.

### Performance

- Provider resolver caches inflectors per culture.
- Prefer specific resource keys.
- Cache commonly used `TranslationOptions`.
- Reuse `ITranslator` in loops; the system is thread-safe.

### API summary

| Interface | Purpose | Access |
|-----------|---------|--------|
| `ITranslator` | Stateless translator (explicit culture) | DI / `LocalizationContext.Translator` |
| `ITranslationService` | Culture-aware translation | DI / `LocalizationContext.TranslationService` |
| `ILocalizationProviderResolver` | Resolve culture-specific providers | DI / `LocalizationContext.Providers` |
| `ILocalizationProviderContext` | Scoped resolver for a fixed culture | `resolver.ForCulture(culture)` |
| `ILocalizationDiagnostics` | Diagnostics and validation | DI / `LocalizationContext.Diagnostics` |
| `ICultureFallbackPolicy` | Pluggable culture chain fallback | `GlobalizationOptions.CultureFallbackPolicy` |
| `ICultureContext` | Read-only application culture | DI / `ICultureService` |

---

## Related packages

- [MyNet.Text](foundations.md)
- [MyNet.Humanizer](humanizer.md)
- [MyNet.Observable](observable.md)
- [MyNet.Fakers](fakers.md)

## Package README

[MyNet.Globalization README](../../src/MyNet.Globalization/README.md)
