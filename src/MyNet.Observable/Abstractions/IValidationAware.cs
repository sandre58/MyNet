// -----------------------------------------------------------------------
// <copyright file="IValidationAware.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public interface IValidationAware : INotifyDataErrorInfo
{
    /// <summary>
    /// Gets a collection of validation error messages associated with the object. This collection is read-only and should be updated by the implementation of the Validate and ValidateProperty methods.
    /// </summary>
    IReadOnlyCollection<string> Errors { get; }

    /// <summary>
    /// Performs validation of the object's state and updates the error collection accordingly.
    /// </summary>
    /// <returns>True if the object is valid; otherwise, false.</returns>
    bool Validate();

    /// <summary>
    /// Validates a specific property of the object and updates the error collection accordingly.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    void ValidateProperty(string propertyName);

    /// <summary>
    /// Resets the validation state of the object by clearing all validation errors.
    /// </summary>
    void ResetValidation();
}
