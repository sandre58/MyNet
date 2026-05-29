# MyNet.Http

HTTP client helpers and Web API consumption utilities on `HttpClient` and `System.Text.Json`.

[![MIT License](https://img.shields.io/github/license/sandre58/MyNet)](https://github.com/sandre58/MyNet/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/MyNet.Http)](https://www.nuget.org/packages/MyNet.Http)

**Target framework:** .NET 10

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

## Related packages

- [MyNet.Text](https://www.nuget.org/packages/MyNet.Text)

## Documentation

- [HTTP clients guide](https://github.com/sandre58/MyNet/blob/main/docs/guides/http.md)
- [Documentation index](https://github.com/sandre58/MyNet/blob/main/docs/index.md)

## License

MIT — see [LICENSE](https://github.com/sandre58/MyNet/blob/main/LICENSE).
