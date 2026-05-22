# MyNet.Observable.Metadata.Generator.Tests

Tests dedicated to `MetadataProviderGenerator`.

## Scope

- verifies source emission for `ObservableMetadataInitializer` + `MetadataApplicators`
- verifies strict fail-fast diagnostic (`MNETMETA001`) when `[assembly: EnforceGeneratedMetadata]` is enabled and an `ObservableObject` has no generated provider

## Run

```powershell
dotnet test D:\repos\github\sandre58\MyNet2\tests\MyNet.Observable.Metadata.Generator.Tests\MyNet.Observable.Metadata.Generator.Tests.csproj -c Debug
```

