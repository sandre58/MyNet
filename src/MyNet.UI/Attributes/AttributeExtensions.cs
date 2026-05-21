// -----------------------------------------------------------------------
// <copyright file="AttributeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;

namespace MyNet.UI.Attributes;

public static class AttributeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Determines whether the specified type is registered as transient in the dependency injection container by checking for the presence of the IsTransientAttribute.
        /// </summary>
        /// <returns><see langword="true"/> if the type is registered as transient; otherwise, <see langword="false"/>.</returns>
        public bool IsRegisteredAsTransient() => type.GetCustomAttributes<IsTransientAttribute>().Any();

        /// <summary>
        /// Determines whether the specified type is registered as a singleton in the dependency injection container by checking for the presence of the IsSingletonAttribute.
        /// </summary>
        /// <returns><see langword="true"/> if the type is registered as a singleton; otherwise, <see langword="false"/>.</returns>
        public bool IsRegisteredAsSingleton() => type.GetCustomAttributes<IsSingletonAttribute>().Any();
    }
}
