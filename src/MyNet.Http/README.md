<div align="center">

# MyNet.Http

<img src="../../assets/MyNetHttp.png" alt="MyNet.Http" width="96" height="96" />

*HTTP client helpers and Web API consumption utilities on HttpClient and System.Text.Json.*

</div>

<div align="center">

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Http)](https://www.nuget.org/packages/MyNet.Http)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

</div>

---

## Features

| Feature | Description |
| :------ | :---------- |
| **WebApiService** | Typed REST calls with System.Text.Json |
| **Timeouts** | Per-request timeout configuration |
| **Requests** | HttpClient-friendly building patterns |
| **Responses** | Helpers for Web API consumption |

---

## Installation

```bash
dotnet add package MyNet.Http
```

## Quick start

```csharp
using MyNet.Http;

public record ItemDto(string Name);

await using var api = new WebApiService(
    new Uri("https://api.example.com/"),
    timeout: TimeSpan.FromSeconds(30));

var item = await api.GetDataAsync<ItemDto>("/items/1", CancellationToken.None);
```

Per-request timeout: `request.SetTimeout(TimeSpan.FromSeconds(2))`.




---
## Related packages

- [MyNet.Text](https://www.nuget.org/packages/MyNet.Text)




---
## Documentation

- [HTTP clients guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/http.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)
---

<div align="center">

<sub>

Copyright © 2016-2026 - Stéphane ANDRE. All Rights Reserved.

<br/>

Released under the [MIT License](https://github.com/sandre58/MyNet/blob/main/LICENSE).

</sub>

</div>
