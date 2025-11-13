// -----------------------------------------------------------------------
// <copyright file="ExtendedObservableCollection.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;
using MyNet.Utilities.Collections;

namespace MyNet.Observable.Collections;

public class ExtendedObservableCollection<T> : OptimizedObservableCollection<T>, IObservableCollection<T>, IExtendedList<T>
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
