# MyNet Translation System - Usage Guide & Architecture

## Executive Summary

The MyNet translation system is a modern, composable localization framework that separates concerns between pure translation logic and contextual concerns. It supports multiple languages, display styles, cultures, and includes advanced features like inflection fallback and dynamic resource registration.

## Architecture Overview

### Core Components

#### 1. Pure Translator (`ITranslator`)
**Responsibility**: Stateless, culture-explicit translation
**Key Characteristic**: No side effects, fully testable without setup

```csharp
public interface ITranslator
{
    string Translate(string key, TranslationOptions options, CultureInfo culture, string? resourceKey = null);
}
```

**Usage Pattern**:
```csharp
var result = translator.Translate("greeting", options, new CultureInfo("en-US"));
```

**When to Use**:
- Server-side where culture is explicit
- Testing translations for specific cultures
- Sharing translation logic across different contexts

#### 2. Contextual Service (`ITranslationService`)
**Responsibility**: Wraps translator with culture context
**Key Characteristic**: Culture comes from environment, ergonomic API

```csharp
public interface ITranslationService
{
    string Translate(string key, TranslationOptions options);
}
```

**Usage Pattern**:
```csharp
// Culture is implicit from context
var result = service.Translate("greeting", options);
```

**When to Use**:
- UI where current culture is contextual
- Reducing parameter passing
- Simplifying API surface

### Key Concepts

#### Display Style
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

#### Culture Fallback Chain
Automatic resolution order when translation not found is governed by `ICultureFallbackPolicy`:

- **`CultureFallbackPolicies.ParentCulture`** (default): en-US → en → invariant
- **`CultureFallbackPolicies.None`**: no fallback (exact match only)
- **Custom policy**: implement `ICultureFallbackPolicy.GetFallback(CultureInfo)`

Configure via `GlobalizationOptions.CultureFallbackPolicy` (DI) or directly on `LocalizationProviderFactoryBuilder.FallbackPolicy`.

#### Inflection Fallback
Applies grammar rules when direct translation unavailable:
- **Flexible Policy**: Default/Short/Narrow styles - inflection allowed
- **Strict Policy**: Symbol/Abbreviation - no inflection

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

#### Simple Translation
```csharp
var options = TranslationOptionsPresets.Default;
var text = service.Translate("greeting", options);
```

#### With Quantity (Pluralization)
```csharp
var options = new TranslationOptionsBuilder()
    .WithQuantity(5)
    .Build();

var text = service.Translate("items", options);
// "items" -> automatically pluralized based on count
```

#### With Display Style
```csharp
var options = new TranslationOptionsBuilder()
    .WithStyle(DisplayStyle.Abbreviation)
    .Build();

var text = service.Translate("currency", options);
// "currency" -> gets abbreviation form (USD, EUR, etc.)
```

#### With Named Arguments
```csharp
var options = new TranslationOptionsBuilder()
    .WithArgument("name", "Alice")
    .WithArgument("count", 42)
    .Build();

var text = service.Translate("message", options);
// Template like "{name} has {count} items"
// Result: "Alice has 42 items"
```

#### Complex Scenario
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

## Template Rendering

Templates use `{placeholder}` or `{placeholder:format}` syntax:

```csharp
// Template in resource:
// "You have {quantity} items worth {amount:C2}"

var options = new TranslationOptionsBuilder()
    .WithQuantity(5)
    .WithArgument("amount", 99.99m)
    .Build();

var result = service.Translate("order_summary", options);
// Result: "You have 5 items worth $99.99" (en-US)
// Result: "You have 5 items worth 99,99 €" (fr-FR)
```

### Built-in Arguments
- **quantity**: Automatically added when `WithQuantity()` is called
- **Custom**: Any arguments via `WithArgument()` or `WithArguments()`

## Translation Resource Structure

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

> **Note**: In DI scenarios prefer `services.AddTranslationResource(nameof(UIResources), UIResources.ResourceManager)`
> which contributes resources during container build-up.

## Humanization Integration

### ListFormatter - Format lists with conjunctions

```csharp
var formatter = LocalizationContext.Providers.Get<IListFormatter>(culture);
// or, throwing if not found:
var formatter = LocalizationContext.Providers.GetRequired<IListFormatter>(culture);
// or, using a scoped context for a specific culture:
var ctx = LocalizationContext.Providers.ForCulture(culture);
var formatter = ctx.GetRequired<IListFormatter>();

// Default: "apple and banana and cherry"
var result1 = formatter.Format(new[] { "apple", "banana", "cherry" });

// With options
var result2 = formatter.Format(
    new[] { "apple", "banana", "cherry" },
    new ListFormattingOptions
    {
        Conjunction = ListConjunction.Or,
        UseOxfordComma = true
    });
// Result: "apple, banana, or cherry"
```

### TimeHumanizer - Human-readable time expressions

```csharp
var humanizer = LocalizationContext.Providers.GetRequired<ITimeHumanizer>(culture);

// Returns culture-specific relative time
var result = humanizer.HumanizeRelativeTime(2, TimeUnit.Hour, Tense.Past);
// en-US: "2 hours ago"
// fr-FR: "il y a 2 heures"
```

### DisplayNameProvider - Humanize enums and SmartEnums

```csharp
// For enums
var provider = LocalizationContext.Providers.Get<IDisplayNameProvider<MyEnum>>(culture);
var displayName = provider.GetDisplayName(MyEnum.Active);

// With style
var shortForm = provider.GetDisplayName(
    MyEnum.Active,
    new DisplayNameOptions { Style = DisplayStyle.Abbreviation });
```

## Display Styles in Practice

### Currency Example
| Key | Default | Short | Abbreviation | Symbol | Narrow |
|-----|---------|-------|--------------|--------|--------|
| currency | Dollar | USD | USD | $ | $ |

### Status Example
| Key | Default | Short | Abbreviation | Symbol | Narrow |
|-----|---------|-------|--------------|--------|--------|
| status_active | Active | Act | ACT | ✓ | ✓ |
| status_inactive | Inactive | Inact | INACT | ✗ | ✗ |

## Pluralization Rules

Supported via language-specific inflectors:

### English Rules
```
0 → "zero"
1 → "one"  
2+ → plural form
```

### French Rules
```
0,1 → singular
2+ → plural
```

### German Rules
```
0,1 → singular
2+ → plural
```

## Best Practices

### 1. Use `TranslationService` in UI
**Why**: Culture is implicit, cleaner API
```csharp
// ✅ Good - UI context
@inject ITranslationService translator
var text = translator.Translate("greeting", options);

// ❌ Less ideal - requires explicit culture
var text = _translator.Translate("greeting", options, currentCulture);
```

### 2. Use `ITranslator` in Core Logic
**Why**: Explicit, testable, composable
```csharp
// ✅ Good - explicit, testable
public class OrderService(ITranslator translator)
{
    public string GetOrderSummary(Order order, CultureInfo culture)
    {
        var options = BuildOptions(order);
        return translator.Translate("order_summary", options, culture);
    }
}
```

### 3. Cache Built Options
**Why**: Avoid rebuilding same configuration
```csharp
// ✅ Good
private static readonly TranslationOptions _defaultOptions = 
    new TranslationOptionsBuilder().Build();

public void UseDefault()
{
    var text = service.Translate("key", _defaultOptions);
}

// ❌ Less efficient - rebuilds every time
public void UsePoorly()
{
    var text = service.Translate("key", 
        new TranslationOptionsBuilder().Build());
}
```

### 4. Use Resource-Specific Lookups
**Why**: Faster lookup, clearer intent
```csharp
// ✅ Good - specific resource
var text = translator.Translate("greeting", options, culture, 
    nameof(UIResources));

// ⚠️ Slower - searches all resources
var text = translator.Translate("greeting", options, culture);
```

### 5. Leverage Inflection for Grammar
**Why**: Automatic pluralization based on quantity
```csharp
// ✅ Good - inflection enabled for flexible styles
var options = new TranslationOptionsBuilder()
    .WithQuantity(count)
    .WithStyle(DisplayStyle.Default)
    .UseInflectionFallback()
    .Build();

// ❌ Avoid for strict styles
var options = new TranslationOptionsBuilder()
    .WithQuantity(count)
    .WithStyle(DisplayStyle.Symbol)
    .UseInflectionFallback()  // Won't apply to symbol
    .Build();
```

### 6. Culture-Aware Formatting
**Why**: Respects user's locale
```csharp
// Template: "Total: {amount:C2}"
var options = new TranslationOptionsBuilder()
    .WithArgument("amount", 1234.56m)
    .Build();

var en_US = service.Translate("total", options);
// Result: "Total: $1,234.56"

var fr_FR = service.Translate("total", options);
// Result: "Total: 1 234,56 €"
```

### 7. Handle Missing Translations Gracefully
**Why**: Improves user experience
```csharp
// Enable fallback to key (recommended)
var options = new TranslationOptionsBuilder()
    .UseKeyFallback()
    .Build();
// Missing key → returns key itself

// Or handle explicitly
var text = translator.Translate("greeting", options, culture);
if (text == "greeting")
{
    // Handle missing translation
    text = "Hello (untranslated)";
}
```

## Advanced Scenarios

### Conditional Display Styles
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

### Dynamic Resource Registration
```csharp
// Register plugin-specific resources at runtime
var catalog = LocalizationContext.Catalog;
catalog.Register("PluginXResources", PluginXResources.ResourceManager);

// Now available for translation
var text = translator.Translate("plugin_greeting", options, culture, "PluginXResources");
```

### Culture-Aware List Formatting
```csharp
var formatter = LocalizationContext.Localizer.GetProvider<IListFormatter>(culture);

var items = new[] { "apple", "banana", "cherry" };
var text = formatter.Format(items, new ListFormattingOptions
{
    Conjunction = ListConjunction.And,
    UseOxfordComma = culture.Name.StartsWith("en") // English uses Oxford comma
});
```

## Troubleshooting

### Translation Returns Key Instead of Translation
**Possible Causes**:
1. Key not found in resource
2. Culture not registered
3. Resource module not registered

**Solution**:
```csharp
// 1. Verify key exists
// 2. Check a provider for the culture is available:
var provider = LocalizationContext.Providers.Get<IInflector>(requestedCulture);

// 3. Verify resource registered:
var hasResource = catalog.Resources.ContainsKey(resourceKey);

// 4. Use diagnostics service for a full report (DI mode only):
var report = LocalizationContext.Diagnostics?.GetDiagnosticReport();
```

### Quantity Not Pluralizing
**Possible Causes**:
1. `UseInflectionFallback` is false
2. Style is Abbreviation/Symbol (Strict policy)
3. Language inflector not configured

**Solution**:
```csharp
// Enable inflection
var options = new TranslationOptionsBuilder()
    .WithQuantity(count)
    .UseInflectionFallback()  // ← Ensure this is true
    .Build();

// Verify inflector for language
var inflector = resolver.Get<IInflector>(culture);
Assert.NotNull(inflector);
```

### Culture Fallback Not Working
**Possible Cause**: `CultureFallbackPolicy` is set to `CultureFallbackPolicies.None`

**Solution**:
```csharp
// In GlobalizationOptions (DI configuration):
services.AddLocalization(/* ... */);
// Default policy is CultureFallbackPolicies.ParentCulture — walks up the hierarchy

// To disable fallback:
services.AddGlobalization(opt => opt.CultureFallbackPolicy = CultureFallbackPolicies.None);

// Custom policy:
services.AddGlobalization(opt => opt.CultureFallbackPolicy = myCustomPolicy);
```

## Performance Considerations

### Caching
- Provider resolver caches inflectors per culture
- Use specific resource keys for faster lookup
- Cache commonly used options

### Large-Scale Operations
```csharp
// ✅ Efficient - reuse translator
var translator = LocalizationContext.Translator;
foreach (var item in largeList)
{
    var text = translator.Translate(item.Key, options, culture);
    // Use translated text
}

// ❌ Inefficient - recreates each time
foreach (var item in largeList)
{
    var newTranslator = new Translator(...);
    var text = newTranslator.Translate(item.Key, options, culture);
}
```

### Concurrent Usage
The system is fully thread-safe:
- Pure translator can be called concurrently
- Provider resolver is thread-safe
- Catalog uses immutable snapshots

## Summary

The MyNet translation system provides:
✅ Clean separation between core logic and context  
✅ Full support for multiple languages and cultures  
✅ Sophisticated pluralization and inflection  
✅ Display style variations for different UI contexts  
✅ High performance with proper caching  
✅ Thread-safe concurrent usage  
✅ Extensible with custom resolvers and providers  
✅ `ILocalizationProviderContext` — culture-scoped provider access without repeating the culture  
✅ `ILocalizationDiagnostics` — built-in diagnostics and validation service  
✅ `ICultureFallbackPolicy` — pluggable culture fallback strategy

Use it for building globalized applications with minimal boilerplate!

### Key API Summary

| Interface | Purpose | Access |
|-----------|---------|--------|
| `ITranslator` | Stateless translator (explicit culture) | DI / `LocalizationContext.Translator` |
| `ITranslationService` | Culture-aware translation (uses app culture) | DI / `LocalizationContext.TranslationService` |
| `ILocalizationProviderResolver` | Resolve culture-specific providers | DI / `LocalizationContext.Providers` |
| `ILocalizationProviderContext` | Scoped resolver for a fixed culture | `resolver.ForCulture(culture)` |
| `ILocalizationDiagnostics` | Diagnostics and validation | DI / `LocalizationContext.Diagnostics` |
| `ICultureFallbackPolicy` | Pluggable culture chain fallback | `GlobalizationOptions.CultureFallbackPolicy` |
| `ICultureContext` | Read-only application-level culture | DI / ICultureService |


