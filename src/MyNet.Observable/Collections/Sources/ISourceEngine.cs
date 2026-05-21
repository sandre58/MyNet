// -----------------------------------------------------------------------
// <copyright file="ISourceEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Defines an interface for a source engine that combines the functionalities of both a source reader and a source writer, allowing for reading from and writing to a data source while also managing resources effectively through the IDisposable interface.
/// </summary>
/// <typeparam name="T">The type of items managed by the source engine.</typeparam>
public interface ISourceEngine<T> : ISourceReader<T>, IDisposable
    where T : notnull;
