// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Text.Json;
using MyNet.Tools.PackageIconGenerator;
using SkiaSharp;
using Svg.Skia;

const int size = 128;
const int pad = 8;
const float footerHeight = 24f;
const float cornerRadius = 18f;
const float iconMaxSize = 56f;

var toolsDir = findToolsDirectory();
var manifestPath = Path.Combine(toolsDir, "package-icons.json");
var svgDir = Path.Combine(toolsDir, "icon-svgs");
var assetsDir = Path.GetFullPath(Path.Combine(toolsDir, "..", "assets"));

if (!File.Exists(manifestPath))
{
    Console.Error.WriteLine($"Manifest not found: {manifestPath}");
    return 1;
}

var manifest = JsonSerializer.Deserialize<IconManifest>(
    File.ReadAllText(manifestPath),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

if (manifest?.Icons is not { Count: > 0 })
{
    Console.Error.WriteLine("No icons in manifest.");
    return 1;
}

Directory.CreateDirectory(assetsDir);
var errors = 0;

foreach (var icon in manifest.Icons)
{
    var outPath = Path.Combine(assetsDir, icon.File);
    var hasGlyph = !string.IsNullOrWhiteSpace(icon.Glyph);
    var svgPath = string.IsNullOrWhiteSpace(icon.Svg) ? null : Path.Combine(svgDir, icon.Svg);

    if (!hasGlyph && (svgPath is null || !File.Exists(svgPath)))
    {
        Console.Error.WriteLine($"Missing SVG or glyph for {icon.File}");
        errors++;
        continue;
    }

    try
    {
        renderIcon(icon, svgPath, outPath);
        Console.WriteLine($"Wrote {outPath}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed {icon.File}: {ex}");
        errors++;
    }
}

return errors == 0 ? 0 : 1;

static void renderIcon(IconDefinition icon, string? svgPath, string outPath)
{
    using var surface = SKSurface.Create(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
    var canvas = surface.Canvas;
    canvas.Clear(SKColors.Transparent);

    var card = SKRect.Create(pad, pad, size - (pad * 2), size - (pad * 2));
    var baseColor = parseHex(icon.Base);
    var accentColor = parseHex(icon.Accent);

    using (var bgPaint = new SKPaint())
    {
        bgPaint.Shader = SKShader.CreateLinearGradient(
            new(card.Left, card.Top),
            new(card.Right, card.Bottom),
            [baseColor, accentColor],
            null,
            SKShaderTileMode.Clamp);
        canvas.DrawRoundRect(card, cornerRadius, cornerRadius, bgPaint);
    }

    var footerTop = card.Bottom - footerHeight;
    var footerRect = SKRect.Create(card.Left, footerTop, card.Width, footerHeight);

    using (var footerPaint = new SKPaint())
    {
        footerPaint.Color = new(0, 0, 0, 100);
        footerPaint.IsAntialias = true;
        canvas.Save();
        var clip = new SKPathBuilder();
        clip.AddRoundRect(card, cornerRadius, cornerRadius);
        canvas.ClipPath(clip.Snapshot(), antialias: true);
        canvas.DrawRect(footerRect, footerPaint);
        canvas.Restore();
    }

    using (var dividerPaint = new SKPaint())
    {
        dividerPaint.Color = accentColor.WithAlpha(160);
        dividerPaint.StrokeWidth = 1f;
        dividerPaint.IsAntialias = true;
        canvas.DrawLine(card.Left + 10, footerTop, card.Right - 10, footerTop, dividerPaint);
    }

    var iconAreaBottom = footerTop - 6f;
    var iconCenterY = card.Top + ((iconAreaBottom - card.Top) / 2f);

    if (!string.IsNullOrWhiteSpace(icon.Glyph))
    {
        drawGlyph(canvas, icon.Glyph, iconCenterY, icon.IconScale ?? 1f);
    }
    else if (svgPath is not null)
    {
        using var skSvg = new SKSvg();
        if (skSvg.Load(svgPath) is null || skSvg.Picture is null)
            throw new InvalidOperationException("Could not load SVG.");

        var bounds = skSvg.Picture.CullRect;
        var iconScale = icon.IconScale is > 0 and var s ? s : 1f;
        var scale = (iconMaxSize * iconScale) / Math.Max(bounds.Width, bounds.Height);

        canvas.Save();
        canvas.Translate(size / 2f, iconCenterY);
        canvas.Scale(scale);
        canvas.Translate(-bounds.MidX, -bounds.MidY);
        canvas.DrawPicture(skSvg.Picture);
        canvas.Restore();
    }

    var typeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold)
                   ?? SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                   ?? SKTypeface.Default;
    using var textPaint = new SKPaint();
    textPaint.Color = SKColors.White.WithAlpha(220);
    textPaint.IsAntialias = true;
    var font = new SKFont(typeface, 11f) { Edging = SKFontEdging.Alias };

    var labelY = footerTop + (footerHeight / 2f) + 4f;
    canvas.DrawText(icon.Label, size / 2f, labelY, SKTextAlign.Center, font, textPaint);

    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    File.WriteAllBytes(outPath, data.ToArray());
}

static void drawGlyph(SKCanvas canvas, string glyph, float centerY, float scale)
{
    var typeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold)
                   ?? SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                   ?? SKTypeface.Default;
    var textSize = 22f * scale;
    if (glyph.Length > 4)
        textSize = 18f * scale;

    using var textPaint = new SKPaint();
    textPaint.Color = SKColors.White;
    textPaint.IsAntialias = true;
    var font = new SKFont(typeface, textSize) { Edging = SKFontEdging.Alias, Embolden = true };

    canvas.DrawText(glyph.ToUpperInvariant(), size / 2f, centerY + (textSize * 0.35f), SKTextAlign.Center, font,  textPaint);
}

static string findToolsDirectory()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        var manifest = Path.Combine(dir.FullName, "tools", "package-icons.json");
        if (File.Exists(manifest))
            return Path.Combine(dir.FullName, "tools");

        var local = Path.Combine(dir.FullName, "package-icons.json");
        if (File.Exists(local))
            return dir.FullName;

        dir = dir.Parent;
    }

    throw new FileNotFoundException("Could not find tools/package-icons.json (walk up from app base directory).");
}

static SKColor parseHex(string hex)
{
    hex = hex.TrimStart('#');
    return hex.Length != 6
        ? throw new ArgumentException($"Invalid color: {hex}")
        : new SKColor(
            Convert.ToByte(hex[..2], 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16));
}
