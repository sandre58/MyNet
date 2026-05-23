# Metadata Generation Guide

This guide describes the **single supported pipeline** for configuring observable metadata in MyNet.Observable.

## Pipeline (authoring → application → runtime)

```
Property attributes on ObservableObject subclasses
        ↓ compile time
MyNet.Observable.Metadata.Generator
        ↓ GeneratedMetadataProviders.g.cs (lazy bootstrap per type)
MetadataRegistry.Get(type) → ObservableMetadataBootstrap.Ensure(type)
        ↓ runtime
MetadataBehaviorApplicator.Apply → behaviors
```

### What you declare

Put metadata attributes on **properties** of types that derive from `ObservableObject`:

```csharp
using MyNet.Observable;
using MyNet.Observable.Behaviors.Metadata.Attributes;

public sealed class Person : ObservableObject
{
    [UpdateOnCultureChanged]
    public string Name { get; set; } = string.Empty;

    [IgnoreModificationTracking]
    public int InternalId { get; set; }

    [AlsoValidate(nameof(Email))]
    public string ConfirmEmail { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [ForwardProperty]
    public AddressViewModel Address { get; set; } = null!;
}
```

Auto-properties are fine when using Fody (`PropertyChanged` / `PropertyChanging` weavers).

### What the generator does

When at least one supported attribute is present on a property of a type that **derives from `ObservableObject`**, the generator emits `ObservableMetadataBootstrap` with one `Configure_{TypeName}()` method per type. Configuration runs **lazily** on the first `MetadataRegistry.Get(type)` for that type (no `[ModuleInitializer]`). Each configure method invokes `MetadataApplicators` on the relevant `PropertyMetadata` entries.

Inspect generated code under `obj/` → `GeneratedMetadataProviders.g.cs`.

Types that are not `ObservableObject` descendants are **not** generated (even if they carry metadata attributes).

### What consumes metadata at runtime

- `MetadataRegistry.Get` — ensures generated configuration for the type, then returns `TypeMetadata`
- Features: `ModificationTrackingFeature`, `EventReactionFeature`, `ValidationDependencyFeature`, `PropertyChangedForwardingFeature`, …
- `MetadataBehaviorApplicator.Apply` — reads `PropertyChangedForwardingFeature` from metadata and registers `PropertyChangedForwardingBehavior` (called from `ObservableObject` constructor)

No application startup call is required.

## Supported metadata attributes

| Attribute | Effect |
|-----------|--------|
| `[IgnoreModificationTracking]` | Property excluded from modification tracking |
| `[UpdateOnCultureChanged]` | Refresh when culture changes |
| `[UpdateOnTimeZoneChanged]` | Refresh when time zone changes |
| `[AlsoValidate("OtherProperty")]` | Validation dependency |
| `[ForwardProperty(concatenatePropertyName: true)]` | Relay child `INotifyPropertyChanged` to owner |

## Strict mode (fail-fast)

Add to any file in the assembly (e.g. `AssemblyInfo.cs`):

```csharp
using MyNet.Observable.Behaviors.Metadata.Attributes;

[assembly: EnforceGeneratedMetadata]
```

When enabled, types deriving from `ObservableObject` **without** any generated metadata configuration produce compile-time error `MNETMETA001`.

Use `[ExemptFromGeneratedMetadata]` on types that intentionally have no metadata (abstract bases, markers, infrastructure VMs).

## Manual fluent configuration (secondary)

Use `MetadataRegistry.For<T>()` and `FeaturesExtensions` only when attributes are not possible:

- Third-party types you do not own
- Dynamic scenarios
- Tests

Example:

```csharp
MetadataRegistry.For<MyType>()
    .UpdateOnCultureChanged(nameof(MyType.DisplayName));
```

Do **not** duplicate the same rules with both attributes and manual fluent configuration on the same type.

## Team rules

1. **Author with attributes** on `ObservableObject` properties.
2. **Rely on the generator** for initialization (module initializer → `MetadataApplicators`).
3. **Do not** configure the same property via attributes and manual fluent API.
4. Use **`[assembly: EnforceGeneratedMetadata]`** when you want compile-time coverage.
5. Imperative forwarding: `owner.ForwardProperty(...)` or `MetadataBehaviorApplicator.ApplyForwardProperty` — updates metadata and registers the behavior.
6. Strict-mode opt-out: `[ExemptFromGeneratedMetadata]` on types without metadata attributes.

## Benefits

1. **Type-safe** — mapping attribute → feature is generated at compile time
2. **Performance** — no reflection at startup for normal types
3. **Explicit** — attributes document intent on properties
4. **Fail-fast** — optional strict mode (`MNETMETA001`)
5. **Traceable** — generated initializer is readable in `GeneratedMetadataProviders.g.cs`
