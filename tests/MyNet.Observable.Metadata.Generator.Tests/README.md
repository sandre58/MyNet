# MyNet.Observable.Metadata.Generator.Tests

Tests for **MetadataProviderGenerator**, **ObservablePropertyGenerator**, and **ObservableObjectSetterAnalyzer**.

## Scope

- lazy `ObservableMetadataBootstrap` emission and `MetadataApplicators` calls
- strict fail-fast diagnostic (`MNETMETA001`) with `[assembly: EnforceGeneratedMetadata]` (`MyNet.Observable.Metadata`)
- `[ObservableProperty]` emission (`MNETOBS001`–`003`)
- setter usage diagnostic (`MNETOBS004`)

## Run

```powershell
dotnet test tests/MyNet.Observable.Metadata.Generator.Tests/MyNet.Observable.Metadata.Generator.Tests.csproj -c Debug
```
