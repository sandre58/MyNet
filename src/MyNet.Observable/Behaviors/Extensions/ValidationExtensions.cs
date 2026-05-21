// -----------------------------------------------------------------------
// <copyright file="ValidationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Behaviors;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ValidationExtensions
{
    extension(ObservableObject owner)
    {
        /// <summary>
        /// Validates the current state of the object by checking if it implements the IValidationAware behavior and invoking its Validate method. This extension method provides a convenient way to perform validation checks on an ObservableObject without needing to directly reference the IValidationAware behavior in the calling code. If the object does not implement IValidationAware, this method will return false, indicating that validation cannot be performed.
        /// </summary>
        /// <returns>True if the object is valid; otherwise, false.</returns>
        public bool Validate() => owner.TryGetBehavior<IValidationAware>(out var behavior) && behavior.Validate();

        /// <summary>
        /// Validates the current state of the object by traversing its behavior graph and checking if any behavior in the graph implements the IValidationAware behavior and invoking its Validate method. This extension method provides a convenient way to perform validation checks on an ObservableObject and all of its behaviors without needing to directly reference the IValidationAware behavior in the calling code. If any behavior in the graph does not implement IValidationAware or if any validation check fails, this method will return false, indicating that validation cannot be performed or that the object is not valid.
        /// </summary>
        /// <returns>True if all behaviors in the graph are valid; otherwise, false.</returns>
        public bool ValidateGraph() => ValidationGraphVisitor.Validate(owner);
    }
}
