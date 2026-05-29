# MyNet.Text

String processing: templating, slugification, sanitization, normalization, truncation, redaction, and formatting pipelines.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Text)](https://www.nuget.org/packages/MyNet.Text)

**Target framework:** .NET 10

## Installation

```bash
dotnet add package MyNet.Text
```

## Quick start

```csharp
using System.Globalization;
using MyNet.Text.Extensions;

var slug = "Hello World!".Slugify();
```

```csharp
using System.Globalization;
using MyNet.Text.Templating;

var options = new TextTemplateOptionsBuilder()
    .WithArgument("Name", "Ada")
    .Build();

var text = new TemplateTransform(options).Apply("Hello, {Name}!", CultureInfo.InvariantCulture);
```

## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Generator](https://www.nuget.org/packages/MyNet.Generator)

## Documentation

- [Foundations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/foundations.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
