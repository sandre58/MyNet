// -----------------------------------------------------------------------
// <copyright file="MetadataGenerationGuide.md" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

# Metadata Generation Guide

This guide explains how to use the new metadata generation features in MyNet.Observable.

## Features

### 1. Metadata Configuration Generation

The source generator now emits metadata configuration code directly for the fluent API (`MetadataRegistry.For<T>()`).

#### Usage

```csharp
using MyNet.Observable;
using MyNet.Observable.Behaviors.Metadata.Attributes;

[GenerateFluentMetadata]
public class MyObservableClass : ObservableObject
{
    private string _name;

    [UpdateOnCultureChanged]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private int _age;

    [IgnoreModificationTracking]
    public int Age
    {
        get => _age;
        set => SetProperty(ref _age, value);
    }
}
```

#### Generated Code

When metadata attributes are detected, the generator emits a module initializer that configures the metadata via `MetadataRegistry`.

It calls methods like:
- `metadata.UpdateOnCultureChanged()`
- `metadata.IgnoreModificationTracking()`
- `metadata.UpdateOnTimeZoneChanged()`
- `metadata.Validates(...)`

### 2. Strict Metadata Enforcement (Fail-Fast Mode)

The `EnforceGeneratedMetadataAttribute` enables compile-time diagnostics to catch missing metadata configurations.

#### Usage

Add this to your `AssemblyInfo.cs` or any `.cs` file in your assembly:

```csharp
using MyNet.Observable.Behaviors.Metadata.Attributes;

[assembly: EnforceGeneratedMetadata]
```

#### Behavior

When strict mode is enabled:
- The generator checks for any `ObservableObject` derived types without generated metadata
- Compile-time diagnostics are emitted for missing configurations
- This ensures all your observable types have proper metadata initialization

## Supported Metadata Attributes

The following attributes are supported for code generation:

- `[IgnoreModificationTracking]` - Property should not be tracked for modifications
- `[UpdateOnCultureChanged]` - Property updates when culture changes
- `[UpdateOnTimeZoneChanged]` - Property updates when timezone changes
- `[AlsoValidate("PropertyName")]` - Property depends on another property for validation

## Example: Complete Setup

```csharp
// In AssemblyInfo.cs or any file
using MyNet.Observable.Behaviors.Metadata.Attributes;

[assembly: EnforceGeneratedMetadata]

namespace MyApp;

// In your class file
using MyNet.Observable;
using MyNet.Observable.Behaviors.Metadata.Attributes;

public class Person : ObservableObject
{
    private string _name;
    private DateTime _birthDate;
    private string _email;

    [UpdateOnCultureChanged]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    [IgnoreModificationTracking]
    [AlsoValidate(nameof(Email))]
    public DateTime BirthDate
    {
        get => _birthDate;
        set => SetProperty(ref _birthDate, value);
    }

    [AlsoValidate(nameof(BirthDate))]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }
}
```

## Benefits

1. **Type-Safe**: Metadata is generated at compile-time, ensuring correctness
2. **Performance**: Metadata is initialized via module initializers, not reflection
3. **Explicit**: Clear declarations via attributes show intent
4. **Fail-Fast**: Strict mode catches missing metadata during compilation
5. **Maintainable**: Generated code is readable and easy to trace

## Implementation Details

- Fluent configuration uses `MetadataRegistry.Get()` to access mutable metadata
- Extensions from `FeaturesExtensions` are used to configure properties fluently
- Module initializers ensure configuration happens before any metadata access

> Note: `GenerateFluentMetadataAttribute` can remain in existing code for backward compatibility,
> but metadata generation is now unified on `MetadataRegistry`.

