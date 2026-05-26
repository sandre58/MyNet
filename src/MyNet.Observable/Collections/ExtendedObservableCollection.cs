// -----------------------------------------------------------------------
// <copyright file="ExtendedObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;
using MyNet.Collections;

namespace MyNet.Observable.Collections;

/// <summary>
/// Represents an extended observable collection that provides additional functionalities and optimizations for managing a collection of items. This class extends the capabilities of a standard ObservableCollection by implementing the IObservableCollection and IExtendedList interfaces, allowing for more efficient data manipulation and change tracking.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class ExtendedObservableCollection<T> : ObservableRangeCollection<T>, IObservableCollection<T>, IExtendedList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedObservableCollection{T}"/> class.
    /// </summary>
    public ExtendedObservableCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedObservableCollection{T}"/> class with initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity to pre-allocate.</param>
    public ExtendedObservableCollection(int capacity)
        : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedObservableCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection from which the elements are copied.</param>
    public ExtendedObservableCollection(Collection<T> collection)
        : base(collection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedObservableCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection from which the elements are copied.</param>
    public ExtendedObservableCollection(IEnumerable<T> collection)
        : base(collection) { }
}
