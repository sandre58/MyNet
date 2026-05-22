# MyNet Coding Conventions

# General Principles

Code should prioritize:
- readability
- predictability
- maintainability
- performance
- explicitness
- consistency

Avoid clever or overly abstract code unless it provides significant architectural value.

---

# C# Language Rules

## Language Version

Use the latest stable C# language features when they improve:
- readability
- maintainability
- safety
- performance

Modern C# features are encouraged.

---

## File-Scoped Namespaces

Always use file-scoped namespaces.

Preferred:

```csharp
namespace MyNet.Utilities;
```

Avoid:

```csharp
namespace MyNet.Utilities
{
}
```

---

## Implicit Typing

Use `var` only when the type is obvious from the right side.

Preferred:

```csharp
var stream = new MemoryStream();
```

Avoid:

```csharp
var value = GetComplexResult();
```

---

## Access Modifiers

Always explicitly specify accessibility.

Avoid implicit private members.

Preferred:

```csharp
private readonly IService _service;
```

---

## Class Design

Prefer:
- sealed classes
- immutable types
- readonly fields
- explicit responsibilities

Avoid deep inheritance hierarchies.

Favor composition over inheritance.

---

## Primary Constructors

Primary constructors are encouraged when they improve readability.

Avoid overly complex primary constructors.

---

## Records

Use records only for:
- immutable data
- value-like objects
- transport models

Do not use records for mutable services or entities with complex lifecycle behavior.

---

# Naming Conventions

## General Naming

Names must be:
- explicit
- descriptive
- intention-revealing

Avoid abbreviations unless universally understood.

---

## Interfaces

Interfaces must start with `I`.

Examples:
- `IFormatter`
- `ILocalizationProvider`

---

## Generic Type Parameters

Use meaningful generic parameter names whenever possible.

Preferred:

```csharp
TValue
TItem
TMetadata
```

Avoid overly generic names like:
- `T1`
- `T2`

---

## Async Methods

Async methods must end with `Async`.

Examples:
- `LoadAsync`
- `SaveAsync`

---

## Private Fields

Private readonly fields should use `_camelCase`.

Example:

```csharp
private readonly IServiceProvider _serviceProvider;
```

---

# Nullability

Nullable reference types must always be enabled.

Avoid null-forgiving operators (`!`) unless absolutely necessary.

Prefer:
- explicit validation
- guard clauses
- safe APIs

---

# Exceptions

Use exceptions only for exceptional scenarios.

Validation should occur early.

Exception messages should:
- be explicit
- contain actionable information
- avoid vague wording

Preferred:

```csharp
throw new ArgumentException("The metadata type must implement IMetadataProvider.", nameof(type));
```

---

# Collections

Prefer:
- `IReadOnlyList<T>`
- `IReadOnlyCollection<T>`
- immutable collections

Avoid exposing mutable collections publicly.

---

# Allocation Rules

Avoid:
- unnecessary closures
- delegate allocations
- boxing
- repeated allocations
- temporary arrays

Prefer:
- cached delegates
- spans
- pooling
- static lambdas

---

# Thread Safety

Thread safety must be explicit.

Shared state must use:
- immutable collections
- concurrent collections
- synchronization primitives when required

Avoid hidden threading assumptions.

---

# Reflection Usage

Reflection should be minimized.

Avoid reflection:
- in hot paths
- during repeated execution
- when compile-time generation is possible

Prefer:
- source generators
- static registries
- strongly typed APIs

---

# Source Generator Conventions

Generators must:
- use incremental generators only
- minimize semantic model usage
- avoid unnecessary allocations
- generate deterministic output

Generated code must:
- compile without warnings
- support nullable reference types
- support AOT and trimming
- remain readable

---

# Dependency Injection

Services should:
- have explicit dependencies
- use constructor injection
- avoid service locator patterns

Avoid hidden dependencies.

---

# Static APIs

Static state should be minimized.

Static APIs are acceptable only for:
- immutable infrastructure
- optional facades
- caching
- singleton abstractions

---

# Extension Methods

Extension methods should:
- remain focused
- avoid hidden side effects
- provide discoverable APIs

Avoid creating utility dumping grounds.

---

# Documentation

Public APIs must have XML documentation.

Documentation should explain:
- purpose
- usage
- constraints
- important behaviors

Avoid redundant comments.

---

# Comments

Comments should explain:
- why
- architectural intent
- non-obvious behavior

Avoid comments that simply restate the code.

Preferred:

```csharp
// Cache delegates to avoid repeated allocations in hot paths.
```

Avoid:

```csharp
// Increment i.
i++;
```

---

# Testing Conventions

Tests should:
- be deterministic
- isolate behavior
- avoid hidden dependencies
- use explicit assertions

Prefer:
- small focused tests
- readable test names
- explicit arrange/act/assert structure

---

# API Design

APIs should:
- be explicit
- avoid magic behavior
- favor strong typing
- minimize ambiguity

Avoid boolean parameter ambiguity.

Preferred:

```csharp
Create(options);
```

Avoid:

```csharp
Create(true, false);
```

---

# Architectural Boundaries

Modules must not:
- introduce circular dependencies
- leak implementation details
- depend unnecessarily on higher-level modules

Shared abstractions belong in lower-level packages.

---

# Preferred Design Patterns

Preferred patterns:
- strategy
- provider
- registry
- factory
- composition
- immutable configuration

Avoid unnecessary abstraction layers.

---

# Code Style Goals

Code should feel:
- simple
- explicit
- modern
- high-performance
- framework-quality
- production-ready