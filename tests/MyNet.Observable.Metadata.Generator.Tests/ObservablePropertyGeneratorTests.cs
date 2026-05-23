// -----------------------------------------------------------------------
// <copyright file="ObservablePropertyGeneratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace MyNet.Observable.Metadata.Generator.Tests;

public sealed class ObservablePropertyGeneratorTests
{
    [Fact]
    public void GeneratesProperty_WithSetPropertySetter()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Metadata;

                              namespace Demo;

                              public partial class SampleVm : ObservableObject
                              {
                                  [ObservableProperty]
                                  private string _title = string.Empty;
                              }
                              """;

        var generated = RunGenerator(source);

        Assert.Contains("public string Title", generated, StringComparison.Ordinal);
        Assert.Contains("set => SetProperty(ref _title, value);", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("ModuleInitializer", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void ReportsDiagnostic_WhenTypeIsNotPartial()
    {
        const string source = """
                              using MyNet.Observable;
                              using MyNet.Observable.Metadata;

                              namespace Demo;

                              public sealed class SampleVm : ObservableObject
                              {
                                  [ObservableProperty]
                                  private int _count;
                              }
                              """;

        var result = RunGenerator(source, out var diagnostics);

        Assert.Contains(diagnostics, d => d.Id == "MNETOBS001");
        Assert.DoesNotContain("SetProperty", result, StringComparison.Ordinal);
    }

    private static string RunGenerator(string source) => RunGenerator(source, out _);

    private static string RunGenerator(string source, out ImmutableArray<Diagnostic> diagnostics)
    {
        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new ObservablePropertyGenerator());
        driver = driver.RunGenerators(compilation);
        var result = driver.GetRunResult();

        diagnostics = [..result.Results.SelectMany(static r => r.Diagnostics)];

        return string.Join(Environment.NewLine, result.Results.SelectMany(static r => r.GeneratedSources).Select(static x => x.SourceText.ToString()));
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new(LanguageVersion.Preview));

        return CSharpCompilation.Create(
            assemblyName: "ObservablePropertyGeneratorTests",
            syntaxTrees: [syntaxTree],
            references: MetadataProviderGeneratorTests.GetMetadataReferences(),
            options: new(OutputKind.DynamicallyLinkedLibrary));
    }
}
