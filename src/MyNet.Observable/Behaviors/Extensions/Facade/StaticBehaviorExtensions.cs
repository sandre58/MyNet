// -----------------------------------------------------------------------
// <copyright file="StaticBehaviorExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Facade;
using MyNet.Observable.Behaviors;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class StaticBehaviorExtensions
{
    extension(ObservableObject obj)
    {
        /// <summary>
        /// Adds localization behavior to the specified observable object using the current globalization services. This method registers a new instance of <see cref="CultureChangedBehavior"/> with the observable object, allowing it to react to culture and time zone changes by updating specific properties. By calling this method on an observable object, you can easily add localization support to it, enabling it to update its display when the culture or time zone changes without modifying the original class or creating a new derived class.
        /// </summary>
        /// <returns>The observable object with localization behavior added.</returns>
        public ObservableObject ReactOnCultureChanged()
        {
            obj.ReactOnCultureChanged(GlobalizationServices.Current);

            return obj;
        }

        /// <summary>
        /// Adds time zone localization behavior to the specified observable object using the current globalization services. This method registers a new instance of <see cref="TimeZoneChangedBehavior"/> with the observable object, allowing it to react to time zone changes by updating specific properties. By calling this method on an observable object, you can easily add time zone localization support to it, enabling it to update its display when the time zone changes without modifying the original class or creating a new derived class.
        /// </summary>
        /// <returns>The observable object with time zone localization behavior added.</returns>
        public ObservableObject ReactOnTimeZoneChanged()
        {
            obj.ReactOnTimeZoneChanged(GlobalizationServices.Current);

            return obj;
        }
    }
}
