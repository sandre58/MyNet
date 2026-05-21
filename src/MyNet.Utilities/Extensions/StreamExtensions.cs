// -----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Library code should not capture the synchronization context.")]
public static class StreamExtensions
{
    private const int DefaultBufferSize = 81920;

    extension(Stream stream)
    {
        /// <summary>
        /// Writes the specified byte array to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        public void WriteBytes(byte[] buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            if (buffer.Length == 0)
                return;

            stream.Write(buffer);
        }

        /// <summary>
        /// Writes the specified byte array to the stream asynchronously.
        /// </summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async ValueTask WriteBytesAsync(
            byte[] buffer,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            if (buffer.Length == 0)
                return;

            await stream
                .WriteAsync(buffer, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the entire stream into a byte array.
        /// </summary>
        /// <returns>The stream content as a byte array.</returns>
        public byte[] ReadAllBytes()
        {
            if (stream is MemoryStream memoryStream &&
                memoryStream.TryGetBuffer(out var segment))
            {
                return segment.Array is not null
                    ? segment.Array.AsSpan(0, (int)memoryStream.Length).ToArray()
                    : memoryStream.ToArray();
            }

            using var ms = new MemoryStream();

            stream.CopyTo(ms, DefaultBufferSize);

            return ms.ToArray();
        }

        /// <summary>
        /// Reads the entire stream into a byte array asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stream content as a byte array.</returns>
        public async ValueTask<byte[]> ReadAllBytesAsync(
            CancellationToken cancellationToken = default)
        {
            if (stream is MemoryStream memoryStream &&
                memoryStream.TryGetBuffer(out var segment))
            {
                return segment.Array is not null
                    ? segment.Array.AsSpan(0, (int)memoryStream.Length).ToArray()
                    : memoryStream.ToArray();
            }

            await using var ms = new MemoryStream();

            await stream
                .CopyToAsync(ms, DefaultBufferSize, cancellationToken)
                .ConfigureAwait(false);

            return ms.ToArray();
        }

        /// <summary>
        /// Writes the specified string to the stream using UTF8 encoding.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        public void WriteString(
            string value,
            Encoding? encoding = null,
            bool leaveOpen = true)
        {
            ArgumentNullException.ThrowIfNull(value);

            using var writer = new StreamWriter(
                stream,
                encoding ?? Encoding.UTF8,
                DefaultBufferSize,
                leaveOpen);

            writer.Write(value);
            writer.Flush();
        }

        /// <summary>
        /// Writes the specified string to the stream asynchronously using UTF8 encoding.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async ValueTask WriteStringAsync(
            string value,
            Encoding? encoding = null,
            bool leaveOpen = true,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(value);

            await using var writer = new StreamWriter(
                stream,
                encoding ?? Encoding.UTF8,
                DefaultBufferSize,
                leaveOpen);

            await writer
                .WriteAsync(value.AsMemory(), cancellationToken)
                .ConfigureAwait(false);

            await writer
                .FlushAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the entire stream as a string.
        /// </summary>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        /// <returns>The stream content as a string.</returns>
        public string ReadAsString(
            Encoding? encoding = null,
            bool leaveOpen = true)
        {
            using var reader = new StreamReader(
                stream,
                encoding ?? Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                DefaultBufferSize,
                leaveOpen);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Reads the entire stream as a string asynchronously.
        /// </summary>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stream content as a string.</returns>
        public async ValueTask<string> ReadAsStringAsync(
            Encoding? encoding = null,
            bool leaveOpen = true,
            CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(
                stream,
                encoding ?? Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                DefaultBufferSize,
                leaveOpen);

            return await reader
                .ReadToEndAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Serializes an object as XML into the stream.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        public void WriteXml<T>(
            T value,
            bool leaveOpen = true)
        {
            ArgumentNullException.ThrowIfNull(value);

            var serializer = XmlSerializerCache<T>.Instance;

            var settings = new XmlWriterSettings { Async = false, Indent = true, Encoding = Encoding.UTF8, CloseOutput = !leaveOpen };

            using var writer = XmlWriter.Create(stream, settings);

            serializer.Serialize(writer, value);
        }

        /// <summary>
        /// Serializes an object as XML into the stream asynchronously.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async ValueTask WriteXmlAsync<T>(
            T value,
            bool leaveOpen = true,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(value);

            var serializer = XmlSerializerCache<T>.Instance;

            var settings = new XmlWriterSettings { Async = true, Indent = true, Encoding = Encoding.UTF8, CloseOutput = !leaveOpen };

            await using var writer = XmlWriter.Create(stream, settings);

            serializer.Serialize(writer, value);

            await writer
                .FlushAsync()
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Deserializes an object from XML.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        /// <returns>The deserialized object.</returns>
        public T? ReadXml<T>(bool leaveOpen = true)
        {
            var serializer = XmlSerializerCache<T>.Instance;

            var settings = new XmlReaderSettings { Async = false, CloseInput = !leaveOpen };

            using var reader = XmlReader.Create(stream, settings);

            return serializer.Deserialize(reader) is T result
                ? result
                : default;
        }

        /// <summary>
        /// Deserializes an object from XML asynchronously.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="leaveOpen">Whether to leave the stream open.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The deserialized object.</returns>
        public async ValueTask<T?> ReadXmlAsync<T>(
            bool leaveOpen = true,
            CancellationToken cancellationToken = default)
        {
            var serializer = XmlSerializerCache<T>.Instance;

            var settings = new XmlReaderSettings { Async = true, CloseInput = !leaveOpen };

            using var reader = XmlReader.Create(stream, settings);

            await reader
                .MoveToContentAsync()
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return serializer.Deserialize(reader) is T result
                ? result
                : default;
        }

        /// <summary>
        /// Rewinds the stream to the beginning if seekable.
        /// </summary>
        public void Rewind()
        {
            if (!stream.CanSeek)
                return;

            stream.Position = 0;
        }
    }

    private static class XmlSerializerCache<T>
    {
        public static readonly XmlSerializer Instance =
            new(typeof(T));
    }
}
