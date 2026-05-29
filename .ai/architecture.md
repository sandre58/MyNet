# MyNet Architecture

## Overview

MyNet is a modular .NET framework composed of reusable libraries focused on:

- utilities
- globalization
- localization
- metadata systems
- object observability
- fake data generation
- source generation
- compile-time infrastructure
- high-performance runtime abstractions

The framework is designed primarily for:
- reusable NuGet packages
- desktop applications
- cross-platform applications
- framework and library development

The architecture prioritizes:
- modularity
- performance
- compile-time safety
- extensibility
- maintainability
- low allocations
- AOT compatibility
- trimming compatibility

---

# Architectural Principles

## Modular Architecture

Each package must have a single clear responsibility.

Modules should:
- remain loosely coupled
- expose minimal public APIs
- avoid unnecessary dependencies
- avoid circular references

Dependencies must flow in a single direction.

---

## Compile-Time First

The framework strongly prefers compile-time mechanisms over runtime reflection.

Preferred techniques:
- source generators
- incremental generators
- static registries
- generated metadata
- strongly typed APIs

Runtime reflection should be avoided whenever possible.

Reflection is acceptable only when:
- no compile-time alternative exists
- startup-only scenarios are involved
- extensibility absolutely requires it

---

## Performance-Oriented Design

Performance is a core architectural concern.

The framework should:
- minimize allocations
- avoid unnecessary LINQ in hot paths
- avoid boxing
- avoid hidden allocations
- minimize reflection
- minimize locking
- favor immutable structures where appropriate

Performance-sensitive APIs should favor:
- spans
- pooling
- caching
- static delegates
- allocation-free enumeration

---

## Thread Safety

Shared registries and caches must be thread-safe.

Preferred techniques:
- immutable collections
- concurrent collections
- lock-free patterns when appropriate

Thread safety must be explicit and predictable.

---

## Public API Stability

Public APIs are considered long-term contracts.

Breaking changes should be minimized.

Public APIs should:
- be consistent
- be discoverable
- use strong typing
- avoid ambiguous behaviors
- avoid magic strings when possible

All public APIs should be documented.

---

## Dependency Injection

The framework uses Microsoft.Extensions.DependencyInjection abstractions.

Modules should:
- expose explicit registration extensions
- avoid service locator patterns
- avoid hidden dependencies
- keep registrations deterministic

Static access should only exist for:
- optional facades
- convenience APIs
- immutable global services

---

# Project Organization

## Core Modules

### MyNet.Utilities

Contains:
- low-level abstractions
- helpers
- collections
- threading primitives
- reflection helpers
- observable infrastructure
- metadata infrastructure
- shared internal utilities

This module should remain lightweight and dependency-minimal.

---

### MyNet.Utilities.Globalization

Contains:
- culture management
- culture scopes
- formatting services
- globalization abstractions
- localization abstractions
- translation infrastructure
- culture-aware services
- localized providers

Globalization services should support:
- scoped culture changes
- async flows
- thread-safe usage

---

### MyNet.Fakers

Contains:
- fake data generation
- localized fake providers
- domain faker infrastructure
- deterministic randomization abstractions

The faker system should:
- support culture-aware generation
- remain extensible
- support dependency injection
- avoid hardcoded global behaviors

---

# Source Generators

Source generators are a major architectural component.

Generators should:
- use incremental generators only
- minimize semantic model usage
- support trimming and AOT
- generate deterministic code
- avoid runtime reflection fallback

Generated code must:
- be nullable-enabled
- be analyzable
- be debuggable
- avoid unnecessary complexity

## Observable metadata

- **Author** with attributes on `ObservableObject` properties.
- **Apply** at compile time via `MyNet.Observable.Metadata.Generator` (lazy `ObservableMetadataBootstrap` on first `MetadataRegistry.Get` → `MetadataApplicators`).
- **Observable properties**: `[ObservableProperty]` on a partial backing field → generated property using `SetProperty`; manual setters call `SetProperty` directly. No Fody.
- **Runtime behaviors** via `MetadataBehaviorApplicator` reading `MetadataRegistry` (including `PropertyChangedForwardingFeature`).
- Strict mode: `[assembly: EnforceGeneratedMetadata]`; opt-out with `[ExemptFromGeneratedMetadata]`.
- Manual `MetadataRegistry.For<T>()` / `MetadataApplicators` is for exceptions only; see `docs/guides/observable.md` (Metadata generation section).

---

# Extensibility

The framework is designed to be extensible.

Extension points should favor:
- interfaces
- strategy patterns
- registries
- providers
- explicit contracts

Behavior should remain predictable and composable.

---

# Error Handling

Exceptions should:
- provide actionable messages
- avoid vague failures
- avoid swallowing errors

Validation should happen as early as possible.

---

# Testing Strategy

The framework prioritizes:
- unit testing
- deterministic behavior
- isolated tests
- generator testing
- performance testing for critical paths

Public APIs and generators should have dedicated tests.

---

# Long-Term Goals

The framework aims to:
- reduce runtime reflection
- maximize compile-time safety
- improve AOT compatibility
- minimize startup overhead
- provide highly reusable infrastructure
- remain framework-agnostic whenever possible