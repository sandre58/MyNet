// -----------------------------------------------------------------------
// <copyright file="ObservableObjectSetterAnalyzerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace MyNet.Observable.Metadata.Generator.Tests;

public sealed class ObservableObjectSetterAnalyzerTests
{
    [Fact]
    public async Task ReportsDiagnostic_WhenSetterAssignsWithoutSetPropertyAsync()
    {
        const string source = """
                              using MyNet.Observable;

                              namespace Demo;

                              public sealed class BadVm : ObservableObject
                              {
                                  private int _value;

                                  public int Value
                                  {
                                      get => _value;
                                      set
                                      {
                                          var before = _value;
                                          _value = value;
                                          OnPropertyChanged(nameof(Value), before, value);
                                      }
                                  }
                              }
                              """;

        var (compilation, diagnostics) = await GetAnalyzerDiagnosticsAsync(source);

        Assert.DoesNotContain(compilation.GetDiagnostics(), static d => d.Severity == DiagnosticSeverity.Error);
        Assert.Contains(diagnostics, static d => d.Id == "MNETOBS004");
    }

    [Fact]
    public async Task NoDiagnostic_WhenSetterUsesSetPropertyAsync()
    {
        const string source = """
                              using MyNet.Observable;

                              namespace Demo;

                              public sealed class GoodVm : ObservableObject
                              {
                                  private int _value;

                                  public int Value
                                  {
                                      get => _value;
                                      set => SetProperty(ref _value, value);
                                  }
                              }
                              """;

        var (_, diagnostics) = await GetAnalyzerDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "MNETOBS004");
    }

    [Fact]
    public async Task NoDiagnostic_WhenSetterUsesManualChangingPipelineAsync()
    {
        const string source = """
                              using MyNet.Observable;

                              namespace Demo;

                              public sealed class ManualVm : ObservableObject
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

        var (_, diagnostics) = await GetAnalyzerDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "MNETOBS004");
    }

    private static async Task<(CSharpCompilation Compilation, ImmutableArray<Diagnostic> Diagnostics)> GetAnalyzerDiagnosticsAsync(string source)
    {
        var compilation = CreateCompilation(source);
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new ObservableObjectSetterAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(true);
        return (compilation, diagnostics);
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new(LanguageVersion.Preview));

        return CSharpCompilation.Create(
            assemblyName: "ObservableObjectSetterAnalyzerTests",
            syntaxTrees: [syntaxTree],
            references: MetadataProviderGeneratorTests.GetMetadataReferences(),
            options: new(OutputKind.DynamicallyLinkedLibrary));
    }
}
