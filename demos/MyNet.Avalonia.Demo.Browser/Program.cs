// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Skia;

namespace MyNet.Avalonia.Demo.Browser;

internal static class Program
{
    private static Task Main() => BuildAvaloniaApp()
        .WithInterFont()
        .UseSkia()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
