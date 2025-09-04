// -----------------------------------------------------------------------
// <copyright file="ProfileMapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace MyNet.AutoMapper.Extensions;

public static class ProfileMapper<TProfile>
    where TProfile : Profile, new()
{
    public static TDestination Map<TSource, TDestination>(TSource source)
        => new MapperConfiguration(cfg => cfg.AddProfile<TProfile>(), NullLoggerFactory.Instance).CreateMapper().Map<TDestination>(source);

    public static TDestination UpdateFrom<TSource, TDestination>(TDestination destination, TSource source)
        => new MapperConfiguration(cfg => cfg.AddProfile<TProfile>(), NullLoggerFactory.Instance).CreateMapper().Map(source, destination);

    public static void UpdateFrom<TSource, TDestination>(
        ICollection<TDestination> destination,
        IEnumerable<TSource>? source,
        Func<TSource, TDestination, bool> predicate,
        Action<TSource>? add = null,
        Action<TDestination>? remove = null)
        where TSource : class
        where TDestination : class
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
        toUpdate.ForEach(x => x.UpdateFrom(sourceList.First(y => y.Equals(x))));

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
