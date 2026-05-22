// -----------------------------------------------------------------------
// <copyright file="MetadataProviderGeneratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MyNet.Observable.Behaviors.Metadata.Attributes;
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
                                          OnPropertyChanging(nameof(Title), before, value);
                                          _title = value;
                                          OnPropertyChanged(nameof(Title), before, value);
                                      }
                                  }
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.Contains("ObservableMetadataInitializer", generated, StringComparison.Ordinal);
        Assert.Contains("Configure_", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataRegistry.Get", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataApplicators.ApplyUpdateOnCultureChanged", generated, StringComparison.Ordinal);
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
                                          OnPropertyChanging(nameof(Title), before, value);
                                          _title = value;
                                          OnPropertyChanged(nameof(Title), before, value);
                                      }
                                  }
                              }

                              """;

        var compilation = CreateCompilation(source);
        var result = RunGenerator(compilation);

        var generated = string.Join(Environment.NewLine, result.Results[0].GeneratedSources.Select(x => x.SourceText.ToString()));

        Assert.Contains("ObservableMetadataInitializer", generated, StringComparison.Ordinal);
        Assert.Contains("Configure_", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataRegistry.Get", generated, StringComparison.Ordinal);
        Assert.Contains("MetadataApplicators.ApplyUpdateOnCultureChanged", generated, StringComparison.Ordinal);
        Assert.DoesNotContain(result.Results[0].Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void ReportsStrictDiagnostic_WhenObservableObjectHasNoGeneratedProvider()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

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
                                          OnPropertyChanging(nameof(Title), before, value);
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
                              using MyNet.Observable.Behaviors.Metadata.Attributes;

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
                                          OnPropertyChanging(nameof(Wrapper), before, value);
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
                                          OnPropertyChanging(nameof(Wrapper), before, value);
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

    private static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new(LanguageVersion.Preview));

        return CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: [syntaxTree],
            references: GetMetadataReferences(),
            options: new(OutputKind.DynamicallyLinkedLibrary));
    }

    private static ImmutableArray<MetadataReference> GetMetadataReferences()
    {
        var assemblies = new[] { typeof(object).Assembly, typeof(Enumerable).Assembly, typeof(List<>).Assembly, typeof(GCSettings).Assembly, typeof(ObservableObject).Assembly, typeof(UpdateOnCultureChangedAttribute).Assembly, typeof(MetadataRegistry).Assembly };

        return
        [
            ..
            assemblies
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .DistinctBy(r => r.Display, StringComparer.OrdinalIgnoreCase)
        ];
    }
}
