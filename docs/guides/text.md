# Text

**Package:** [MyNet.Text](../../src/MyNet.Text/README.md)

`MyNet.Text` provides reusable text transformations for casing, templating, slugification, sanitization, normalization, truncation, redaction, and formatting.

Most APIs are available as string extension methods and as composable transforms through `TextPipeline`.

## Quick start

```csharp
using MyNet.Text;

var slug = "Hello World!".Slugify();
var title = "hello world".ToTitleCase();
```

For fluent composition, use `Text.For(...)`:

```csharp
using System.Globalization;
using MyNet.Text;
using MyNet.Text.Normalization;
using MyNet.Text.Slugification;

var result = Text.For("  Cafe du Monde!  ", CultureInfo.InvariantCulture)
    .Normalize(TextNormalization.CleanWhitespace)
    .Normalize(TextNormalization.RemoveDiacritics)
    .Slugify(Slugifier.KebabCase)
    .Value;
// "cafe-du-monde"
```

---

## Available transformations

| Category | Main extensions | Predefined transforms |
|---------|------------------|-----------------------|
| Casing | `ToUpperCase()`, `ToLowerCase()`, `ToTitleCase()`, `ToSentenceCase()`, `ToPascalCase()`, `ToCamelCase()`, `ToSnakeCase()`, `ToKebabCase()` | `Casing.*` |
| Normalization | `Normalize(...)`, `NormalizeUnicode(...)`, `NormalizeWhitespace()`, `RemoveDiacritics()` | `TextNormalization.*` |
| Slugification | `Slugify(...)` | `Slugifier.Default`, `Slugifier.KebabCase`, `Slugifier.SnakeCase`, `Slugifier.PreserveCase` |
| Sanitization | `Sanitize(...)`, `SanitizeFileName()` | `Sanitizer.FileName`, `Sanitizer.AlphaNumeric`, `Sanitizer.Identifier`, `Sanitizer.UrlSegment` |
| Truncation | `Truncate(...)`, `TruncateWords(...)`, `TruncateCharacters(...)` | `Truncator.*` |
| Redaction | `Redact(...)` | `Redactor.Generic`, `Redactor.Email`, `Redactor.Phone`, `Redactor.CardNumber` |
| Templating | `ResolveTemplate(...)` | `TextTemplating.Default`, `TextTemplating.Create(...)` |
| Formatting | `Initials()`, `HumanizeKey()`, `Increment(...)`, `IncrementAlpha(...)` | `Formatter.Initials`, `Formatter.HumanizeKey` |
| Randomization | `RandomizePattern()`, `FitLength(...)`, `ReplaceSymbolsByNumbers(...)` | `TextRandomGenerator.Current` |
| URI/validation | `ToRelativeUri(...)`, `ToWebUri(...)`, `IsEmailAddress()`, `IsPhoneNumber()` | `RegexPatterns.*` |

---

## Casing

```csharp
using MyNet.Text;
using MyNet.Text.TextCasing;

var upper = "hello world".ToUpperCase();
var lower = "Hello World".ToLowerCase();
var title = "welcome to mynet".ToTitleCase();
var sentence = "hELLO wORLD".ToSentenceCase();
var pascal = "order item name".ToPascalCase();
var camel = "order item name".ToCamelCase();
var snake = "Order Item Name".ToSnakeCase();
var kebab = "Order Item Name".ToKebabCase();
var enumBased = "hello world".ApplyCase(LetterCasing.Upper);
```

| Input | API | Output |
|------|-----|--------|
| `"hello world"` | `ToUpperCase()` | `"HELLO WORLD"` |
| `"Hello World"` | `ToLowerCase()` | `"hello world"` |
| `"welcome to mynet"` | `ToTitleCase()` | `"Welcome To Mynet"` |
| `"hELLO wORLD"` | `ToSentenceCase()` | `"Hello world"` |
| `"order item name"` | `ToPascalCase()` | `"OrderItemName"` |
| `"order item name"` | `ToCamelCase()` | `"orderItemName"` |
| `"Order Item Name"` | `ToSnakeCase()` | `"order_item_name"` |
| `"Order Item Name"` | `ToKebabCase()` | `"order-item-name"` |
| `"hello world"` | `ApplyCase(LetterCasing.Upper)` | `"HELLO WORLD"` |

Some culture-sensitive casing rules can differ by culture and runtime (for example Turkish `i/I` rules).

---

## Normalization

```csharp
using System.Text;
using MyNet.Text;

var clean = "  Hello   world  ".NormalizeWhitespace();
var ascii = "Cr\u00E8me br\u00FBl\u00E9e".RemoveDiacritics();
var formC = "e\u0301".NormalizeUnicode(NormalizationForm.FormC);
```

| Input | API | Output |
|------|-----|--------|
| `"  Hello   world  "` | `NormalizeWhitespace()` | `"Hello world"` |
| `"Cr\u00E8me br\u00FBl\u00E9e"` | `RemoveDiacritics()` | `"Creme brulee"` |
| `"e\u0301"` | `NormalizeUnicode(FormC)` | `"\u00E9"` |

---

## Slugification

```csharp
using MyNet.Text;
using MyNet.Text.Slugification;

var defaultSlug = "Cr\u00E8me Brulee Recipe".Slugify();
var snakeSlug = "Cr\u00E8me Brulee Recipe".Slugify(Slugifier.SnakeCase);
var keepCase = "My Product V2".Slugify(Slugifier.PreserveCase);
```

| Input | API | Output |
|------|-----|--------|
| `"Cr\u00E8me Brulee Recipe"` | `Slugify()` | `"creme-brulee-recipe"` |
| `"Cr\u00E8me Brulee Recipe"` | `Slugify(Slugifier.SnakeCase)` | `"creme_brulee_recipe"` |
| `"My Product V2"` | `Slugify(Slugifier.PreserveCase)` | `"My-Product-V2"` |

---

## Sanitization

```csharp
using MyNet.Text;
using MyNet.Text.Sanitization;

var fileName = "report:2026/06?.pdf".SanitizeFileName();
var alphaNumeric = "A#B C-42".Sanitize(Sanitizer.AlphaNumeric);
var identifier = "Order #42 - EU".Sanitize(Sanitizer.Identifier);
var urlSegment = "Category 2026/06".Sanitize(Sanitizer.UrlSegment);
```

| Input | API | Output |
|------|-----|--------|
| `"report:2026/06?.pdf"` | `SanitizeFileName()` | Platform-dependent safe filename |
| `"A#B C-42"` | `Sanitize(Sanitizer.AlphaNumeric)` | `"ABC42"` |
| `"Order #42 - EU"` | `Sanitize(Sanitizer.Identifier)` | `"Order42EU"` |
| `"Category 2026/06"` | `Sanitize(Sanitizer.UrlSegment)` | `"Category202606"` |

`SanitizeFileName()` depends on invalid filename characters of the current platform/system.

---

## Truncation

```csharp
using MyNet.Text;
using MyNet.Text.Truncation;

var fixedLength = "Hello World".Truncate(8, "...");
var fixedWords = "one two three four".TruncateWords(2, "...");
var fixedCharsRight = "ABCDEFGHIJ".TruncateCharacters(4, "...", TruncateFrom.Right);
var fixedCharsLeft = "ABCDEFGHIJ".TruncateCharacters(4, "...", TruncateFrom.Left);
```

| Input | API | Output |
|------|-----|--------|
| `"Hello World"` | `Truncate(8, "...")` | `"Hello..."` |
| `"one two three four"` | `TruncateWords(2, "...")` | `"one two..."` |
| `"ABCDEFGHIJ"` | `TruncateCharacters(4, "...", Right)` | `"ABCD..."` |
| `"ABCDEFGHIJ"` | `TruncateCharacters(4, "...", Left)` | `"...GHIJ"` |

If the text is already shorter than the target length/count, output is unchanged.

---

## Redaction

```csharp
using System.Text.RegularExpressions;
using MyNet.Text;
using MyNet.Text.Redaction;

var generic = "SensitiveValue".Redact(Redactor.Generic);
var card = "4111 1111 1111 1111".Redact(Redactor.CardNumber);
var phone = "+33 6 12 34 56 78".Redact(Redactor.Phone);
var email = "ada@example.com".Redact(Redactor.Email);
var custom = "SensitiveValue".Redact(showStart: 2, showEnd: 2, maskCharacter: '#');
var regex = "token=ABC123".Redact(new Regex("ABC[0-9]+"), "***");
```

| Input | API | Output |
|------|-----|--------|
| `"SensitiveValue"` | `Redact(Redactor.Generic)` | `"Se**********ue"` |
| `"4111 1111 1111 1111"` | `Redact(Redactor.CardNumber)` | `"**** **** **** 1111"` (pattern-based) |
| `"+33 6 12 34 56 78"` | `Redact(Redactor.Phone)` | Digits masked except last two |
| `"ada@example.com"` | `Redact(Redactor.Email)` | Local part and domain core masked (pattern-based) |
| `"SensitiveValue"` | `Redact(2, 2, '#')` | `"Se##########ue"` |
| `"token=ABC123"` | `Redact(Regex("ABC[0-9]+"), "***")` | `"token=***"` |

Regex-based redactors preserve separators/spaces and mainly mask matching characters.

---

## Templates

Named placeholders support optional format strings.

```csharp
using System.Globalization;
using MyNet.Text;

var hello = "Hello, {Name}!".ResolveTemplate([new KeyValuePair<string, object?>("Name", "Ada")]);
var money = "Total: {Amount:C2}".ResolveTemplate(
    [new KeyValuePair<string, object?>("Amount", 42.5m)],
    CultureInfo.GetCultureInfo("en-US"));
var date = "Date: {When:yyyy-MM-dd}".ResolveTemplate(
    [new KeyValuePair<string, object?>("When", new DateTime(2026, 6, 6))],
    CultureInfo.InvariantCulture);
var quantityPrefix = "items".ResolveTemplate(options => options.PrefixWithQuantity(3, quantityFormat: "N0"));
var quantitySuffix = "items".ResolveTemplate(options => options.SuffixWithQuantity(3, quantityFormat: "N0"));
var missing = "Hello, {Unknown}!".ResolveTemplate([]);
```

| Input | API | Output |
|------|-----|--------|
| `"Hello, {Name}!"` | `ResolveTemplate(Name=Ada)` | `"Hello, Ada!"` |
| `"Total: {Amount:C2}"` | `ResolveTemplate(Amount=42.5, en-US)` | `"Total: $42.50"` |
| `"Date: {When:yyyy-MM-dd}"` | `ResolveTemplate(When=2026-06-06)` | `"Date: 2026-06-06"` |
| `"items"` | `PrefixWithQuantity(3, "N0")` | `"3 items"` |
| `"items"` | `SuffixWithQuantity(3, "N0")` | `"items 3"` |
| `"Hello, {Unknown}!"` | `ResolveTemplate([])` | `"Hello, {Unknown}!"` |

Currency and number/date rendering can vary by culture.

---

## Formatting helpers

```csharp
using MyNet.Text;

var initials = "Ada Lovelace".Initials();
var humanized = "CeciEstUnTest".HumanizeKey();

var existingNumeric = new[] { "Document", "Document1", "Document2" };
var incrementNumeric = "Document".Increment(existingNumeric);

var existingAlpha = new[] { "Version", "VersionA", "VersionB" };
var incrementAlpha = "Version".IncrementAlpha(existingAlpha);
```

| Input | API | Output |
|------|-----|--------|
| `"Ada Lovelace"` | `Initials()` | `"AL"` |
| `"CeciEstUnTest"` | `HumanizeKey()` | `"Ceci est un test"` |
| `"ceci_est_un_test"` | `HumanizeKey()` | `"Ceci est un test"` |
| `"HTTPStatusCode404"` | `HumanizeKey()` | `"Http status code 404"` |
| `"Document"` + `["Document", "Document1", "Document2"]` | `Increment(...)` | `"Document3"` |
| `"Version"` + `["Version", "VersionA", "VersionB"]` | `IncrementAlpha(...)` | `"VersionC"` |

---

## Random pattern generation

`RandomizePattern()` turns tokenized patterns into random text.

```csharp
using MyNet.Text;

var id = "INV-{#6}".RandomizePattern();
var code = "??##".RandomizePattern();
var hex = "{[A-F]8}".RandomizePattern();
var fixedLength = "AB".FitLength(min: 5, max: 5);
```

| Input | API | Example output |
|------|-----|----------------|
| `"INV-{#6}"` | `RandomizePattern()` | `"INV-493027"` |
| `"??##"` | `RandomizePattern()` | `"QF42"` |
| `"{[A-F]8}"` | `RandomizePattern()` | `"AFCDBEFA"` |
| `"AB"` | `FitLength(min: 5, max: 5)` | `"AB7K2"` |

Random output is non-deterministic unless you inject a deterministic/random-seeded generator in tests.

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyNet.Text;

var services = new ServiceCollection();
services.AddRandomGenerator();
```

---

## URI and regex helpers

```csharp
using MyNet.Text;

var relative = "api".ToRelativeUri("users", "42");
var web = "https://example.com".ToWebUri(("q", "mynet text"), ("page", "1"));
var isEmail = "ada@example.com".IsEmailAddress();
var isPhone = "+33612345678".IsPhoneNumber();
```

| Input | API | Output |
|------|-----|--------|
| `"api" + "users", "42"` | `ToRelativeUri(...)` | `"api/users/42"` |
| `"https://example.com" + ("q","mynet text"), ("page","1")` | `ToWebUri(...)` | `"https://example.com/?q=mynettext&page=1"` |
| `"ada@example.com"` | `IsEmailAddress()` | `true` |
| `"+33612345678"` | `IsPhoneNumber()` | `true` |

Final URI query encoding/sanitization can vary depending on inputs.

---

## Notes on system-dependent outputs

- `SanitizeFileName()` depends on platform invalid filename rules.
- Culture-aware transformations (`ToTitleCase`, numeric/currency/date template formatting) can vary by culture and runtime.
- `RandomizePattern()` and `FitLength()` return different values on each call unless randomness is controlled.
- Regex-based redaction keeps original non-matching characters, so formatting in output depends on input shape.

---

## Edge cases and exceptions

| Input | API | Output |
|------|-----|--------|
| `""` | `NormalizeWhitespace()` | `""` |
| `"short"` | `Truncate(10, "...")` | `"short"` |
| `"Hello {Missing}"` + no args | `ResolveTemplate([])` | `"Hello {Missing}"` |
| `"api" + ".."` | `ToRelativeUri("..")` | `ArgumentException` |
| `"/relative/path"` | `ToWebUri(("q", "1"))` | `ArgumentException` (base URI must be absolute) |
| `"INV-{#"` | `RandomizePattern()` | `FormatException` (missing `}`) |

Use these cases in tests when you want to validate behavior under invalid inputs.

---

## Related

- [Foundations](foundations.md)
- [Documentation index](../index.md)
- [MyNet.Text README](../../src/MyNet.Text/README.md)



