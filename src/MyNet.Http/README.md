<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <img src="../../assets/MyNetHttp.png" width="128" alt="MyNetHttp">
</div>

<h1 align="center">My .NET - Http</h1>

[![MIT License](https://img.shields.io/github/license/sandre58/mynet?style=for-the-badge)](https://github.com/sandre58/mynet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Http?style=for-the-badge)](https://www.nuget.org/packages/MyNet.Http)

Helpers and extensions for HTTP client operations in .NET applications.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](#)
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-purple)](#)
[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-purple)](#)
[![C#](https://img.shields.io/badge/language-C%23-blue)](#)

---

## Installation

Install via NuGet:

```bash
dotnet add package MyNet.Http
```

## Features

- Typed HTTP client built on `HttpClient` and `System.Text.Json`
- `IWebApiService` abstraction for easier testing and DI
- RFC 7807 problem details parsing via `ProblemDetailsParser`
- Per-request timeout support via `HttpRequestExtensions.SetTimeout`
- GET/POST/PUT/PATCH/DELETE helpers with async-first API

## Example Usage

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Http;

public record MyResponseType(string Value);

await using var api = new WebApiService(
    new Uri("https://api.example.com/"),
    timeout: TimeSpan.FromSeconds(30));

var response = await api.GetDataAsync<MyResponseType>("/data", CancellationToken.None);
await api.PostDataAsync("/items", new { Name = "Sample" }, CancellationToken.None);
```

### Custom HttpClient (tests or DI)

```csharp
using var client = new HttpClient(new MyHandler()) { BaseAddress = new Uri("https://api.example.com/") };
await using var api = new WebApiService(client, timeout: TimeSpan.FromSeconds(10), disposeClient: true);
```

### Per-request timeout

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, "/slow");
request.SetTimeout(TimeSpan.FromSeconds(2));
```

## License

Copyright © Stéphane ANDRE.

Distributed under the MIT License. See [LICENSE](../../LICENSE) for details.
