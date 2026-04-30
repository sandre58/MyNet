// -----------------------------------------------------------------------
// <copyright file="CollectionSourceMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Collections.Sources;

/// <summary>
/// Defines the mode of a collection source, which determines how the collection is accessed and modified. The mode can be one of the following:
/// - OwnedMutable: The collection is owned by the source and can be modified directly.
/// - ReadOnlySnapshot: The collection is a read-only snapshot of the source, and cannot be modified directly. Changes to the source will not affect the snapshot.
/// - ExternalLive: The collection is an external live collection that is not owned by the source, and can be modified directly. Changes to the source will affect the collection, and changes to the collection will affect the source.
/// The mode of a collection source is important for determining how to interact with the collection, and can affect performance and behavior of the source. For example, a read-only snapshot may be more efficient for certain operations, while an owned mutable collection may be more flexible for others. An external live collection may require additional synchronization to ensure consistency between the source and the collection. Understanding the mode of a collection source is essential for effectively using and managing collections in an observable context, and can help to ensure that the source and collection are used in a way that is appropriate for their intended purpose and behavior.
/// </summary>
public enum CollectionSourceMode
{
    OwnedMutable,

    ReadOnlySnapshot,

    ExternalLive
}
