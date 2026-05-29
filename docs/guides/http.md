# HTTP clients

**Package:** [MyNet.Http](../../src/MyNet.Http/README.md)

Typed **REST-style** helpers on `HttpClient` and `System.Text.Json`, with optional per-request timeouts and RFC 7807 problem details parsing.

Depends on **MyNet.Text**.

## Quick start

```csharp
using MyNet.Http;

public record ItemDto(int Id, string Name);

await using var api = new WebApiService(
    new Uri("https://api.example.com/"),
    timeout: TimeSpan.FromSeconds(30));

var item = await api.GetDataAsync<ItemDto>("/items/1", CancellationToken.None);
await api.PostDataAsync("/items", new { Name = "New" }, CancellationToken.None);
```

## Constructing the service

### Base URL + owned client

```csharp
var api = new WebApiService(
    serverUrl: new Uri("https://api.example.com/"),
    timeout: TimeSpan.FromSeconds(30),
    headers: new Dictionary<string, string> { ["Authorization"] = "Bearer token" });
```

### Injected `HttpClient` (recommended for DI)

```csharp
services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
});

// Factory:
var client = httpClientFactory.CreateClient("MyApi");
await using var api = new WebApiService(client, timeout: TimeSpan.FromSeconds(30), disposeClient: false);
```

Set `disposeClient: true` only if the service should dispose the client when disposed.

## API surface (`IWebApiService`)

| Method | Use |
|--------|-----|
| `GetDataAsync<T>` | GET + deserialize JSON body |
| `GetStreamAsync` | GET stream (files, large payloads) |
| `PostDataAsync` | POST JSON body |
| `PutDataAsync` | PUT JSON body |
| `PatchDataAsync` | PATCH JSON body |
| `DeleteDataAsync` | DELETE |
| `SendRequestAsync` | Low-level `HttpRequestMessage` |

Generic constraints and JSON options use **case-insensitive** property names by default.

## Query parameters

Use `ApiParameter` tuples (defined alongside `WebApiService`):

```csharp
ApiParameter[] query = [("page", "1"), ("size", "20")];
await api.GetDataAsync<PagedResult<ItemDto>>("/items", CancellationToken.None, query);
```

## Per-request timeout

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, "/slow-endpoint");
request.SetTimeout(TimeSpan.FromSeconds(5));

await api.SendRequestAsync(request, CancellationToken.None);
```

`SetTimeout` / `GetTimeout` are in `HttpRequestExtensions`.

## Error handling

Failed responses can be turned into exceptions via the optional `toException` delegate on the constructor.

### Problem Details (RFC 7807)

```csharp
using MyNet.Http.Exceptions;

var json = await response.Content.ReadAsStringAsync();
var ex = ProblemDetailsParser.TryParseException(json);
if (ex is not null)
    throw ex;
```

`WebApiService` integrates parsing for API error payloads — see `tests/MyNet.Http.Tests/WebApiServiceTests.cs` (`ProblemDetailsResponseThrowsMultipleHttpExceptionAsync`).

Exception types: `HttpException`, `WebApiException`, `MultipleHttpException`.

## Testing

Use `HttpMessageHandler` mocks:

```csharp
var handler = new StubHandler(HttpStatusCode.OK, """{"id":1,"name":"Test"}""");
using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
await using var service = new WebApiService(client);
var dto = await service.GetDataAsync<ItemDto>("/items/1", CancellationToken.None);
```

See `tests/MyNet.Http.Tests/WebApiServiceTests.cs`.

## Related

- [Foundations → Text](foundations.md#mynettext)
- [Package README](../../src/MyNet.Http/README.md)
