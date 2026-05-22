// -----------------------------------------------------------------------
// <copyright file="BehaviorExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using FluentValidation;
using MyNet.Globalization.Culture;
using MyNet.Globalization.DateTime;
using MyNet.Observable.Behaviors;
using MyNet.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for the <see cref="ObservableObject"/> class, allowing to easily add behaviors to an observable object. This class contains extension methods that can be called on any instance of <see cref="ObservableObject"/> to add specific behaviors, such as localization support, without the need to modify the original class or create a new derived class. By using these extension methods, you can enhance the functionality of your observable objects in a modular and reusable way, making it easier to maintain and extend your codebase.
/// </summary>
public static class BehaviorExtensions
{
    /// <summary>
    /// Adds localization behavior to the specified observable object. This method registers a new instance of <see cref="CultureChangedBehavior"/> with the observable object, allowing it to react to culture and time zone changes by updating specific properties. By calling this method on an observable object, you can easily add localization support to it, enabling it to update its display when the culture or time zone changes without modifying the original class or creating a new derived class.
    /// </summary>
    extension<TOwner>(TOwner owner)
        where TOwner : ObservableObject
    {
        /// <summary>
        /// Adds the specified behavior to the observable object. This method allows you to add any behavior that implements the <see cref="IObservableBehavior"/> interface to the observable object, enabling you to enhance its functionality in a modular and reusable way. By calling this method with a specific behavior, you can easily add new features or capabilities to your observable objects without modifying their original implementation or creating new derived classes.
        /// </summary>
        /// <typeparam name="TBehavior">The type of the behavior to add.</typeparam>
        /// <returns>The observable object with the added behavior.</returns>
        public TOwner Use<TBehavior>()
            where TBehavior : class, IObservableBehavior
        {
            owner.RegisterBehavior((TBehavior)Activator.CreateInstance(typeof(TBehavior), owner)!);

            return owner;
        }

        /// <summary>
        /// Adds localization behavior to the specified observable object.
        /// </summary>
        /// <param name="cultureService">The culture service used to handle culture changes.</param>
        /// <returns>The observable object with localization behavior added.</returns>
        public ObservableObject ReactOnCultureChanged(ICultureService cultureService)
        {
            owner.RegisterBehavior(new CultureChangedBehavior(owner, cultureService));

            return owner;
        }

        /// <summary>
        /// Adds time zone localization behavior to the specified observable object. This method registers a new instance of <see cref="TimeZoneChangedBehavior"/> with the observable object, allowing it to react to time zone changes by updating specific properties. By calling this method on an observable object, you can easily add time zone localization support to it, enabling it to update its display when the time zone changes without modifying the original class or creating a new derived class.
        /// </summary>
        /// <param name="timeZoneService">The time zone service used to handle time zone changes.</param>
        /// <returns>The observable object with time zone localization behavior added.</returns>
        public ObservableObject ReactOnTimeZoneChanged(ITimeZoneService timeZoneService)
        {
            owner.RegisterBehavior(new TimeZoneChangedBehavior(owner, timeZoneService));

            return owner;
        }

        /// <summary>
        /// Adds modification tracking behavior to the specified observable object. This method registers a new instance of <see cref="ModificationTrackingBehavior"/> with the observable object, allowing it to track changes to its properties and collections. By calling this method on an observable object, you can easily add modification tracking support to it, enabling it to keep track of changes and react accordingly without modifying the original class or creating new derived classes.
        /// </summary>
        /// <returns>The observable object with modification tracking behavior added.</returns>
        public ObservableObject UseTracking()
        {
            owner.RegisterBehavior(new ModificationTrackingBehavior(owner));

            return owner;
        }

        /// <summary>
        /// Adds validation behavior to the specified observable object. This method registers a new instance of <see cref="ValidationBehavior{T}"/> with the observable object, allowing it to validate its properties using the specified validator. By calling this method on an observable object, you can easily add validation support to it, enabling it to validate its properties and react accordingly without modifying the original class or creating new derived classes.
        /// </summary>
        /// <param name="validator">The validator used to validate the observable object's properties.</param>
        /// <returns>The validation behavior added to the observable object.</returns>
        public ValidationBehavior<TOwner> UseValidation(IValidator validator)
        {
            var behavior = new ValidationBehavior<TOwner>(owner, validator);

            owner.RegisterBehavior(behavior);

            return behavior;
        }

        /// <summary>
        /// Registers a <see cref="PropertyChangedForwardingBehavior"/> for the property selected by the expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="selector">Expression selecting the property (e.g. x => x.Wrapper).</param>
        /// <param name="concatenatePropertyName">True to emit Wrapper.Name; false to emit Name only.</param>
        /// <returns>The observable object.</returns>
        public ObservableObject ForwardProperty<T>(Expression<Func<TOwner, T?>> selector, bool concatenatePropertyName = true)
            where T : class, INotifyPropertyChanged
        {
            MetadataBehaviorApplicator.ApplyForwardProperty(owner, selector.PropertyName(), concatenatePropertyName);
            return owner;
        }

        /// <summary>
        /// Registers a <see cref="PropertyChangedForwardingBehavior"/> for the provided property name.
        /// </summary>
        /// <param name="propertyName">The wrapper property name.</param>
        /// <param name="concatenatePropertyName">True to emit Wrapper.Name; false to emit Name only.</param>
        /// <returns>The observable object.</returns>
        public ObservableObject ForwardProperty(string propertyName, bool concatenatePropertyName = true)
        {
            MetadataBehaviorApplicator.ApplyForwardProperty(owner, propertyName, concatenatePropertyName);
            return owner;
        }
    }
}
