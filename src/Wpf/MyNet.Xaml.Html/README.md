<div id="top"></div>

<!-- PROJECT INFO -->
<br />
<div align="center">
  <a href="https://github.com/sandre58/MyNetXaml">
    <img src="images/logo.png" width="256" height="256">
  </a>

<h1 align="center">My .NET Xaml</h1>

[![Downloads][downloads-shield]][downloads-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

  <p align="center">
Various tools for easing the development of XAML related applications.

As i only use WPF myself everything is focused on WPF, but things should work for other XAML dialects (at least in theory).

You can either use the commandline tool `MyNet.Xaml.Merger` or the MSBuild version `MyNet.Xaml.Merger.MSBuild` to make use of the provided functionalities.
  </p>

[![Language][language-shield]][language-url]
[![Framework][framework-shield]][framework-url]
[![Version][version-shield]][version-url]
[![Build][build-shield]][build-url]

</div>

## XAMLCombine

Combines multiple XAML files to one large file.  
This is useful when you want to provide one `Generic.xaml` instead of multiple small XAML files.  
Using one large XAML file not only makes it easier to consume, but can also drastically improving loading performance.

### Using the MSBuild-Task

```
<XAMLCombineItems Include="Themes/Controls/*.xaml">
  <TargetFile>Themes/Generic.xaml</TargetFile>
</XAMLCombineItems>
```

The MSBuild-Task includes the items used for combining as pages during debug builds and removes them from pages during release builds.
This is done to reduce the binary size for release builds and still enable intellisense in debug builds for those XAML files.

**Remarks when using Rider**  
To get intellisense in debug builds inside the XAML files and to prevent duplicate display of those files you have to define:

```
<PropertyGroup Condition="'$(IsBuildingInsideRider)' == 'True'">
  <DefaultItemExcludes>$(DefaultItemExcludes);Themes\Controls\*.xaml</DefaultItemExcludes>
</PropertyGroup>

<ItemGroup Condition="'$(IsBuildingInsideRider)' == 'True'">
  <Page Include="Themes\Controls\*.xaml" />
</ItemGroup>
```

### Using the executable

`XAMLTools` accepts the following commandline parameters for the `combine` verb:

- `-s "Path_To_Your_SourceFile"` => A file containing a new line separated list of files to combine (lines starting with # are skipped)
- `-t "Path_To_Your_Target_File.xaml"`

## XAMLColorSchemeGenerator

Generates color scheme XAML files while replacing certain parts of a template file.

For an example on how this tool works see the [generator input](src/MyNet.Xaml.Merger/XAMLColorSchemeGenerator/GeneratorParameters.json) and [template](src/MyNet.Xaml.Merger/XAMLColorSchemeGenerator/ColorScheme.Template.xaml) files.

### Using the MSBuild-Task

```
<XAMLColorSchemeGeneratorItems Include="Themes\ColorScheme.Template.xaml">
  <ParametersFile>Themes\GeneratorParameters.json</ParametersFile>
  <OutputPath>Themes\ColorSchemes</OutputPath>
</XAMLColorSchemeGeneratorItems>
```

### Using the executable

`XAMLTools` accepts the following commandline parameters for the `colorscheme` verb:

- `-p "Path_To_Your_GeneratorParameters.json"`
- `-t "Path_To_Your_ColorScheme.Template.xaml"`
- `-o "Path_To_Your_Output_Folder"`

## License

Copyright © Stéphane ANDRE.

My .NET WPF is provided as-is under the MIT license. For more information see [LICENSE](./LICENSE).

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[language-shield]: https://img.shields.io/github/languages/top/sandre58/MyNetXaml
[language-url]: https://github.com/sandre58/MyNetXaml
[forks-shield]: https://img.shields.io/github/forks/sandre58/MyNetXaml?style=for-the-badge
[forks-url]: https://github.com/sandre58/MyNetXaml/network/members
[stars-shield]: https://img.shields.io/github/stars/sandre58/MyNetXaml?style=for-the-badge
[stars-url]: https://github.com/sandre58/MyNetXaml/stargazers
[issues-shield]: https://img.shields.io/github/issues/sandre58/MyNetXaml?style=for-the-badge
[issues-url]: https://github.com/sandre58/MyNetXaml/issues
[license-shield]: https://img.shields.io/github/license/sandre58/MyNetXaml?style=for-the-badge
[license-url]: https://github.com/sandre58/MyNetXaml/blob/main/LICENSE
[build-shield]: https://img.shields.io/github/actions/workflow/status/sandre58/MyNetXaml/ci.yml?logo=github&label=CI
[build-url]: https://github.com/sandre58/MyNetXaml/actions
[downloads-shield]: https://img.shields.io/github/downloads/sandre58/MyNetXaml/total?style=for-the-badge
[downloads-url]: https://github.com/sandre58/MyNetXaml/releases
[framework-shield]: https://img.shields.io/badge/.NET-8.0-purple
[framework-url]: https://github.com/sandre58/MyNetXaml/tree/main/src/MyNet.Xaml.Merger.MSBuild
[version-shield]: https://img.shields.io/nuget/v/MyNet.Xaml.Merger.MSBuild
[version-url]: https://www.nuget.org/packages/MyNet.Xaml.Merger.MSBuild