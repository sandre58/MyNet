// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyNet.Xaml.Html;

public static class XamlToImageFileService
{
    public static void SaveImage(string xamlFilePath, string filePath) => SaveImage(XamlToBitmapConverter.ToBitmapSource(xamlFilePath), filePath);

    public static void SaveImage(FrameworkElement frameworkElement, string filePath) => SaveImage(XamlToBitmapConverter.ToBitmapSource(frameworkElement), filePath);

    public static void SaveImage(Visual visual, Size size, string filePath) => SaveImage(XamlToBitmapConverter.ToBitmapSource(visual, size), filePath);

    private static void SaveImage(BitmapSource bitmap, string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);

        var encoder = CreateBitmapEncoder(fullPath);
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        using var stream = File.Create(fullPath);
        encoder.Save(stream);
    }

    public static void SaveIcon(string xamlFilePath, string filePath) => SaveIcon(XamlToBitmapConverter.ToBitmapSource(xamlFilePath), filePath);

    public static void SaveIcon(FrameworkElement frameworkElement, string filePath) => SaveIcon(XamlToBitmapConverter.ToBitmapSource(frameworkElement), filePath);

    public static void SaveIcon(Visual visual, Size size, string filePath) => SaveIcon(XamlToBitmapConverter.ToBitmapSource(visual, size), filePath);

    private static void SaveIcon(BitmapSource bitmap, string filePath)
    {
        using var input = ToStream(bitmap);
        SaveIcon(input, filePath);
    }

    public static void SaveIcon(Stream input, string outputFilePath)
    {
        var fullPath = Path.GetFullPath(outputFilePath);

        using var stream = File.Create(fullPath);
        SaveIcon(input, stream);
    }

    private static void SaveIcon(Stream input, Stream output)
    {
        using var bitmap = new System.Drawing.Bitmap(input);
        var handle = bitmap.GetHicon();
        using (var icon = System.Drawing.Icon.FromHandle(handle))
        {
            icon.Save(output);
        }
        _ = DestroyIcon(handle);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    private static MemoryStream ToStream(BitmapSource bitmap)
    {
        var memory = new MemoryStream();

        var encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        encoder.Save(memory);

        return memory;
    }

    private static BitmapEncoder CreateBitmapEncoder(string filePath) => Path.GetExtension(filePath).ToLowerInvariant() switch
    {
        ".bmp" => new BmpBitmapEncoder(),
        ".gif" => new GifBitmapEncoder(),
        ".jpeg" or ".jpg" or ".jpe" => new JpegBitmapEncoder(),
        ".png" => new PngBitmapEncoder(),
        ".tiff" or ".tif" => new TiffBitmapEncoder(),
        ".wdp" or ".hdp" => new WmpBitmapEncoder(),
        _ => throw new ArgumentException("Can not encode bitmaps for the specified file extension.", nameof(filePath)),
    };

}
