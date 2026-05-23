// -----------------------------------------------------------------------
// <copyright file="MetadataProviderGeneratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MyNet.Observable.Behaviors.Metadata.Attributes;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Utilities.Metadata;
using Xunit;

namespace MyNet.Observable.Metadata.Generator.Tests;

public sealed class MetadataProviderGeneratorTests
{
    [Fact]
    public void GeneratesFluentConfigurator_WhenMetadataAttributesArePresent()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

                              namespace Demo;

                              public sealed class Vm : ObservableObject
                              {
                                  private string _title = string.Empty;

                                  [UpdateOnCultureChanged]
                                  public string Title
                                  {
                                      get => _title;
                                      set
                                      {
                                          var before = _title;
                                          if (!OnPropertyChanging(nameof(Title), before, value))
                                              return;
                                          _title = value;
                                          OnPropertyChanged(nameof(Title), before, value);
                                      }
                                  }
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.Contains("ObservableMetadataBootstrap", generated, StringComparison.Ordinal);
        Assert.Contains("internal static void Ensure(Type type)", generated, StringComparison.Ordinal);
        Assert.Contains("Configure_", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataRegistry.Get", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataApplicators.ApplyUpdateOnCultureChanged", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("ModuleInitializer", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("ObservableMetadataInitializer", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("GeneratedMetadataProvider0", generated, StringComparison.Ordinal);
        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void GeneratesFluentConfigurator_ForAutoPropertyWithMetadataAttributes()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

                              namespace Demo;

                              public sealed class Vm : ObservableObject
                              {
                                  private string _title = string.Empty;

                                  [UpdateOnCultureChanged]
                                  public string Title
                                  {
                                      get => _title;
                                      set
                                      {
                                          var before = _title;
                                          if (!OnPropertyChanging(nameof(Title), before, value))
                                              return;
                                          _title = value;
                                          OnPropertyChanged(nameof(Title), before, value);
                                      }
                                  }
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.Contains("ObservableMetadataBootstrap", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("ModuleInitializer", generated, StringComparison.Ordinal);
        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void DoesNotGenerate_WhenTypeIsNotObservableObject()
    {
        const string source = """
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

                              namespace Demo;

                              public sealed class PlainDto
                              {
                                  [UpdateOnCultureChanged]
                                  public string Title { get; set; } = string.Empty;
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.DoesNotContain("Configure_Demo_PlainDto", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("ObservableMetadataBootstrap", generated, StringComparison.Ordinal);
        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void LazyBootstrap_AppliesMetadataOnFirstRegistryGet()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Behaviors.Metadata.Attributes;
                              using MyNet.Observable.Behaviors.Metadata.Features.Events;
                              using MyNet.Utilities.Metadata;

                              namespace Demo;

                              public sealed class Vm : ObservableObject
                              {
                                  [UpdateOnCultureChanged]
                                  public string Title { get; set; } = string.Empty;
                              }

                              """;

        var metadata = CompileAndGetMetadata(source, "Demo.Vm", "Title");

        Assert.True(metadata.TryGetFeature<EventReactionFeature>(out var feature));
        Assert.NotNull(feature);
        Assert.Contains(typeof(CultureChangedEvent), feature.Events);
    }

    [Fact]
    public void ReportsStrictDiagnostic_WhenObservableObjectHasNoGeneratedProvider()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Metadata;

                              [assembly: EnforceGeneratedMetadata]

                              namespace Demo;

                              public sealed class VmWithoutMetadata : ObservableObject
                              {
                                  private string _title = string.Empty;

                                  public string Title
                                  {
                                      get => _title;
                                      set
                                      {
                                          var before = _title;
                                          if (!OnPropertyChanging(nameof(Title), before, value))
                                              return;
                                          _title = value;
                                          OnPropertyChanged(nameof(Title), before, value);
                                      }
                                  }
                              }
                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        Assert.Contains(result.Results[0].Diagnostics, d => d.Id == "MNETMETA001");
    }

    [Fact]
    public void DoesNotReportStrictDiagnostic_WhenTypeIsExempt()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Metadata;

                              [assembly: EnforceGeneratedMetadata]

                              namespace Demo;

                              [ExemptFromGeneratedMetadata]
                              public sealed class BaseVm : ObservableObject
                              {
                              }

                              public sealed class VmWithoutMetadata : BaseVm
                              {
                                  public string Title { get; set; } = string.Empty;
                              }
                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Id == "MNETMETA001");
    }

    [Fact]
    public void GeneratesBehaviorConfiguration_WhenForwardPropertyAttributeIsPresent()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

                              namespace Demo;

                              public sealed class Vm : ObservableObject
                              {
                                  private object _wrapper = new object();

                                  [ForwardProperty]
                                  public object Wrapper
                                  {
                                      get => _wrapper;
                                      set
                                      {
                                          var before = _wrapper;
                                          if (!OnPropertyChanging(nameof(Wrapper), before, value))
                                              return;
                                          _wrapper = value;
                                          OnPropertyChanged(nameof(Wrapper), before, value);
                                      }
                                  }
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.Contains("MetadataApplicators.ApplyForwardProperty(metadata_Wrapper, true);", generated, StringComparison.Ordinal);
        Assert.Contains("var metadata_Wrapper", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("GeneratedPropertyBehaviorRegistry.RegisterForwardProperty", generated, StringComparison.Ordinal);
        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void GeneratesFeatureAndBehaviorConfiguration_WhenBothKindsArePresent()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

                              namespace Demo;

                              public sealed class Vm : ObservableObject
                              {
                                  private object _wrapper = new object();

                                  [IgnoreModificationTracking]
                                  [ForwardProperty]
                                  public object Wrapper
                                  {
                                      get => _wrapper;
                                      set
                                      {
                                          var before = _wrapper;
                                          if (!OnPropertyChanging(nameof(Wrapper), before, value))
                                              return;
                                          _wrapper = value;
                                          OnPropertyChanged(nameof(Wrapper), before, value);
                                      }
                                  }
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.Contains("MetadataApplicators.ApplyIgnoreModificationTracking(metadata_Wrapper);", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataApplicators.ApplyForwardProperty(metadata_Wrapper, true);", generated, StringComparison.Ordinal);
        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    private static GeneratorDriverRunResult RunGenerator(CSharpCompilation compilation)
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new MetadataProviderGenerator());
        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult();
    }

    private static PropertyMetadata CompileAndGetMetadata(string source, string typeFullName, string propertyName)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        var syntaxTrees = new List<SyntaxTree>
        {
            CSharpSyntaxTree.ParseText(source, parseOptions)
        };

        var generated = RunGenerator(CreateCompilation(source));

        syntaxTrees.AddRange(generated.Results[0].GeneratedSources.Select(generatedSource => CSharpSyntaxTree.ParseText(generatedSource.SourceText.ToString(), parseOptions, path: generatedSource.HintName)));

        var compilation = CSharpCompilation.Create(
            assemblyName: "LazyBootstrapTests",
            syntaxTrees: [.. syntaxTrees],
            references: GetMetadataReferences(),
            options: new(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        Assert.True(emitResult.Success, string.Join(Environment.NewLine, emitResult.Diagnostics));

        var assembly = Assembly.Load(stream.ToArray());
        var type = assembly.GetType(typeFullName, throwOnError: true)!;

        return MetadataRegistry.Get(type).GetProperty(propertyName);
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new(LanguageVersion.Preview));

        return CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: [syntaxTree],
            references: GetMetadataReferences(),
            options: new(OutputKind.DynamicallyLinkedLibrary));
    }

    public static ImmutableArray<MetadataReference> GetMetadataReferences()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var references = new List<MetadataReference>();

        addAssembly(typeof(object).Assembly);
        addAssembly(typeof(ObservableObject).Assembly);
        addAssembly(typeof(MetadataRegistry).Assembly);
        addAssembly(typeof(UpdateOnCultureChangedAttribute).Assembly);

        return [.. references];

        void addAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location))
                return;

            if (!paths.Add(assembly.Location))
                return;

            references.Add(MetadataReference.CreateFromFile(assembly.Location));

            foreach (var referenceName in assembly.GetReferencedAssemblies())
            {
                try
                {
                    addAssembly(Assembly.Load(referenceName));
                }
                catch (FileNotFoundException)
                {
                }
            }
        }
    }
}
