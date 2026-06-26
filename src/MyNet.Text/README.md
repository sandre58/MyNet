<div align="center">

# MyNet.Text

<img src="../../assets/MyNetText.png" alt="MyNet.Text" width="96" height="96" />

*String processing: templating, slugification, sanitization, normalization, truncation, redaction, and formatting pipelines.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Text)](https://www.nuget.org/packages/MyNet.Text)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **Templates** | Token-based templates with culture-aware formatting |
| **Transforms** | Slugify, sanitize, normalize, truncate, and redact |
| **Pipelines** | Composable text transformation pipelines |
| **DI extensions** | Random generator registration for tests |

---

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




---
## Related packages

- [MyNet.Primitives](https://www.nuget.org/packages/MyNet.Primitives)
- [MyNet.Generator](https://www.nuget.org/packages/MyNet.Generator)




---
## Documentation

- [Text transformations guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/text.md)
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
