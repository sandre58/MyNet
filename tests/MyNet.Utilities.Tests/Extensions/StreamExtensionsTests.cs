// -----------------------------------------------------------------------
// <copyright file="StreamExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xunit;

namespace MyNet.Utilities.Tests.Extensions;

[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Test code")]
public class StreamExtensionsTests
{
    #region WriteBytes / ReadAllBytes

    [Fact]
    public void WriteBytes_WritesDataToStream()
    {
        using var stream = new MemoryStream();
        var data = new byte[] { 1, 2, 3, 4, 5 };
        stream.WriteBytes(data);
        Assert.Equal(5, stream.Length);
    }

    [Fact]
    public void WriteBytes_EmptyBufferDoesNotWrite()
    {
        using var stream = new MemoryStream();
        stream.WriteBytes([]);
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void WriteBytes_NullBufferThrows()
    {
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => stream.WriteBytes(null!));
    }

    [Fact]
    public void ReadAllBytes_ReadsFromMemoryStream()
    {
        var data = new byte[] { 10, 20, 30 };
        using var stream = new MemoryStream(data);
        var result = stream.ReadAllBytes();
        Assert.Equal(data, result);
    }

    [Fact]
    public void ReadAllBytes_ReadsFromNonMemoryStream()
    {
        var data = new byte[] { 10, 20, 30 };
        using var inner = new MemoryStream(data);

        // Wrap in a non-MemoryStream to exercise the copy branch
        using var stream = new NonSeekableStream(inner);
        var result = stream.ReadAllBytes();
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task WriteBytesAsync_WritesDataToStreamAsync()
    {
        await using var stream = new MemoryStream();
        var data = new byte[] { 1, 2, 3 };
        await stream.WriteBytesAsync(data);
        Assert.Equal(3, stream.Length);
    }

    [Fact]
    public async Task ReadAllBytesAsync_ReadsFromMemoryStreamAsync()
    {
        var data = new byte[] { 5, 6, 7 };
        await using var stream = new MemoryStream(data);
        var result = await stream.ReadAllBytesAsync();
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task ReadAllBytesAsync_ReadsFromNonMemoryStreamAsync()
    {
        var data = new byte[] { 5, 6, 7 };
        await using var inner = new MemoryStream(data);
        await using var stream = new NonSeekableStream(inner);
        var result = await stream.ReadAllBytesAsync();
        Assert.Equal(data, result);
    }

    #endregion

    #region WriteString / ReadAsString

    [Fact]
    public void WriteString_And_ReadAsString_RoundTrip()
    {
        using var stream = new MemoryStream();
        const string text = "Hello, World!";
        stream.WriteString(text);
        stream.Position = 0;
        var result = stream.ReadAsString();
        Assert.Equal(text, result);
    }

    [Fact]
    public void WriteString_NullThrows()
    {
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => stream.WriteString(null!));
    }

    [Fact]
    public async Task WriteStringAsync_And_ReadAsStringAsync_RoundTripAsync()
    {
        await using var stream = new MemoryStream();
        const string text = "Async Hello!";
        await stream.WriteStringAsync(text);
        stream.Position = 0;
        var result = await stream.ReadAsStringAsync();
        Assert.Equal(text, result);
    }

    #endregion

    #region WriteXml / ReadXml

    [Fact]
    public void WriteXml_And_ReadXml_RoundTrip()
    {
        var original = new SampleXmlObject { Name = "Test", Value = 42 };
        using var stream = new MemoryStream();
        stream.WriteXml(original);
        stream.Position = 0;
        var result = stream.ReadXml<SampleXmlObject>();
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void WriteXml_NullThrows()
    {
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => stream.WriteXml<SampleXmlObject>(null!));
    }

    [Fact]
    public async Task WriteXmlAsync_And_ReadXmlAsync_RoundTripAsync()
    {
        var original = new SampleXmlObject { Name = "AsyncTest", Value = 99 };
        await using var stream = new MemoryStream();
        await stream.WriteXmlAsync(original);
        stream.Position = 0;
        var result = await stream.ReadXmlAsync<SampleXmlObject>();
        Assert.NotNull(result);
        Assert.Equal("AsyncTest", result.Name);
        Assert.Equal(99, result.Value);
    }

    #endregion

    #region Rewind

    [Fact]
    public void Rewind_SeekableStream_SetsPositionToZero()
    {
        using var stream = new MemoryStream([1, 2, 3]);
        stream.Position = 2;
        stream.Rewind();
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public void Rewind_NonSeekableStream_DoesNotThrow()
    {
        using var inner = new MemoryStream([1, 2, 3]);
        using var stream = new NonSeekableStream(inner);

        // Should not throw
        stream.Rewind();
    }

    #endregion

    // ── helpers ────────────────────────────────────────────────────────────
    [XmlRoot("Sample")]
    [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Test helper class")]
    [SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Test helper class")]
    public sealed class SampleXmlObject
    {
        public string? Name { get; set; }

        public int Value { get; set; }
    }

    /// <summary>A stream wrapper that hides the CanSeek flag.</summary>
    private sealed class NonSeekableStream(Stream inner) : Stream
    {
        public override bool CanRead => inner.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => inner.CanWrite;

        public override long Length => inner.Length;

        public override long Position { get => inner.Position; set => inner.Position = value; }

        public override void Flush() => inner.Flush();

        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);

        public override void SetLength(long value) => inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);
    }
}
