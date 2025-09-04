// -----------------------------------------------------------------------
// <copyright file="MappingExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace MyNet.AutoMapper.Extensions;

public static class MappingExtensions
{
    public static TDestination MapTo<TSource, TDestination>(this TSource source)
        => new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>(), NullLoggerFactory.Instance).CreateMapper().Map<TDestination>(source);

    public static TDestination UpdateFrom<TSource, TDestination>(this TDestination destination, TSource source)
        => new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>(), NullLoggerFactory.Instance).CreateMapper().Map(source, destination);

    public static void UpdateFrom<TSource, TDestination>(this ICollection<TDestination> destination,
        IEnumerable<TSource>? source,
        Func<TSource, TDestination, bool> predicate,
        Action<TSource>? add = null,
        Action<TDestination>? remove = null)
    {
        if (source is null) return;

        var destinationList = destination.ToList();
        var sourceList = source.ToList();

        // Delete
        var toDelete = destinationList.Where(x => !sourceList.Exists(y => predicate(y, x))).ToList();
        toDelete.ForEach(x =>
        {
            if (remove is not null)
                remove(x);
            else
                destination.Remove(x);
        });

        // Update
        var toUpdate = destinationList.Where(x => sourceList.Exists(y => predicate(y, x))).ToList();
        toUpdate.ForEach(x => x.UpdateFrom(sourceList.First(y => predicate(y, x))));

        // Add
        var toAdd = sourceList.Where(x => !destinationList.Exists(y => predicate(x, y))).ToList();
        toAdd.ForEach(x =>
        {
            if (add is not null)
                add(x);
            else
                destination.Add(x.MapTo<TSource, TDestination>());
        });
    }
}
