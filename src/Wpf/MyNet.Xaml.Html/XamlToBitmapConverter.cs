// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyNet.Xaml.Html;

public static class XamlToBitmapConverter
{
    public static BitmapSource ToBitmapSource(string xamlFilePath)
    {
        if (!File.Exists(xamlFilePath)) throw new IOException($"File does'nt exist : {xamlFilePath}");
        var frameworkElement = (FrameworkElement)Application.LoadComponent(new Uri(xamlFilePath, UriKind.Absolute));
        return ToBitmapSource(frameworkElement);
    }

    public static BitmapSource ToBitmapSource(FrameworkElement frameworkElement)
    {
        frameworkElement.UpdateLayout();
        var bitmap = new RenderTargetBitmap((int)frameworkElement.ActualWidth, (int)frameworkElement.ActualHeight, 96, 96, PixelFormats.Default);
        bitmap.Render(frameworkElement);
        return bitmap;
    }

    public static BitmapSource ToBitmapSource(Visual visual, Size size)
    {
        var bitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Default);
        bitmap.Render(visual);
        return bitmap;
    }
}
