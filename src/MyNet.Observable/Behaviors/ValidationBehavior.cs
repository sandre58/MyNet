// -----------------------------------------------------------------------
// <copyright file="ValidationBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using MyNet.Metadata;
using MyNet.Observable.Behaviors.Metadata.Features;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Provides validation support for an ObservableObject.
/// </summary>
public sealed class ValidationBehavior<T>(T owner, IValidator validator) : SuspendableBehavior<T>(owner), IPropertyChangedBehavior, IValidationBehavior
    where T : ObservableObject
{
    private readonly Dictionary<string, List<string>> _errors = [];

    #region INotifyDataErrorInfo

    /// <inheritdoc/>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Gets all validation errors for the object. This property returns an enumerable collection of all validation error messages that are currently associated with the object. It aggregates the error messages from all properties by selecting the values from the internal dictionary of errors and flattening them into a single collection. By accessing this property, you can retrieve a comprehensive list of all validation issues that are currently affecting the object, which can be useful for displaying error messages to users or for logging purposes.
    /// </summary>
    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "Conforms to INotifyDataErrorInfo interface.")]
    public IReadOnlyCollection<string> Errors => [.. _errors.Values.SelectMany(x => x).Distinct()];

    /// <inheritdoc />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc />
    public IEnumerable GetErrors(string? propertyName)
        => string.IsNullOrWhiteSpace(propertyName)
            ? _errors.SelectMany(x => x.Value)
            : (IEnumerable)(_errors.TryGetValue(propertyName, out var list)
                ? list
                : []);

    #endregion

    #region Validation

    /// <inheritdoc/>
    public bool Validate()
    {
        if (IsDisposed)
            return false;

        if (IsSuspended)
            return true;

        var context = CreateValidationContext();

        var result = validator.Validate(context);

        ApplyValidationResult(result);

        return !HasErrors;
    }

    /// <inheritdoc/>
    public void ValidateProperty(string propertyName)
    {
        if (IsDisposed)
            return;

        if (IsSuspended)
            return;

        var selector = new MemberNameValidatorSelector([propertyName]);
        var context = CreateValidationContext(selector);
        var result = validator.Validate(context);

        ApplyPropertyValidationResult(propertyName, result);
    }

    /// <summary>
    /// Resets the validation state of the object by clearing all validation errors. This method clears the internal dictionary of validation errors, effectively resetting the validation state of the object. It also raises the necessary notifications to indicate that the validation state has changed, allowing any UI elements or other components that are bound to the validation errors to update accordingly. By calling this method, you can clear all existing validation errors and start fresh with a clean slate for validating the object's properties again as needed.
    /// </summary>
    public void ResetValidation()
    {
        if (_errors.Count == 0)
            return;

        var properties = _errors.Keys.ToArray();

        _errors.Clear();

        RaiseValidationStateChanged();

        foreach (var property in properties)
        {
            ErrorsChanged?.Invoke(Owner, new(property));
        }
    }

    #endregion

    #region Property behavior

    /// <inheritdoc />
    public void OnPropertyChanged(PropertyMutationContext context)
    {
        if (IsDisposed || IsSuspended)
            return;

        if (string.IsNullOrWhiteSpace(context.PropertyName))
            return;

        ValidateProperty(context.PropertyName);

        foreach (var dependent in GetDependents(context.PropertyName))
        {
            ValidateProperty(dependent);
        }
    }

    #endregion

    #region Internal validation

    /// <summary>
    /// Creates a validation context for the owner object. This method creates a validation context that is used when performing validation on the owner object. The validation context contains information about the object being validated, as well as any additional settings or selectors that may be needed for the validation process. By creating a validation context, this method allows the validation behavior to provide the necessary context and information to the validator when executing the validation logic, ensuring that the validation rules are applied correctly based on the current state of the owner object and any specified selectors.
    /// </summary>
    /// <param name="selector">The validator selector to use, or null to use the default selector.</param>
    /// <returns>The validation context for the owner object.</returns>
    private ValidationContext<T> CreateValidationContext(IValidatorSelector? selector = null) => new(Owner, new(), selector ?? ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory());

    /// <summary>
    /// Applies the validation result for the entire object. This method takes the validation result for the entire object and updates the internal dictionary of errors accordingly. It groups the validation errors by property name and updates the error messages in the dictionary for each property. If there are no errors for a property, it removes any existing entry for that property from the dictionary. After updating the errors, it raises the necessary notifications to indicate that the validation state has changed and that the errors for each affected property have been updated. By calling this method with the validation result for the entire object, you can ensure that the validation state of all properties is accurately reflected in the internal error tracking and that any UI elements or other components that are bound to the validation errors are properly updated to reflect the current state of validation for all properties of the object.
    /// </summary>
    /// <param name="result">The validation result for the entire object.</param>
    private void ApplyValidationResult(ValidationResult result)
    {
        var previousProperties = _errors.Keys.ToArray();

        _errors.Clear();

        foreach (var group in result.Errors.GroupBy(x => x.PropertyName))
        {
            _errors[group.Key] =
            [
                .. group
                    .Select(x => x.ErrorMessage)
                    .Distinct()
            ];

            RaiseErrorsChanged(group.Key);
        }

        foreach (var property in previousProperties)
        {
            if (!_errors.ContainsKey(property))
                RaiseErrorsChanged(property);
        }

        RaiseValidationStateChanged();
    }

    /// <summary>
    /// Applies the validation result for a specific property. This method takes the validation result for a specific property and updates the internal dictionary of errors accordingly. It checks if there are any validation errors for the specified property and updates the error messages in the dictionary. If there are no errors, it removes any existing entry for that property from the dictionary. After updating the errors, it raises the necessary notifications to indicate that the validation state has changed and that the errors for the specified property have been updated. By calling this method with the validation result for a specific property, you can ensure that the validation state of that property is accurately reflected in the internal error tracking and that any UI elements or other components that are bound to the validation errors are properly updated to reflect the current state of validation for that property.
    /// </summary>
    /// <param name="propertyName">The name of the property for which the validation result is being applied.</param>
    /// <param name="result">The validation result for the specified property.</param>
    private void ApplyPropertyValidationResult(string propertyName, ValidationResult result)
    {
        var errors =
            result.Errors
                .Where(x => x.PropertyName == propertyName)
                .Select(x => x.ErrorMessage)
                .Distinct()
                .ToList();

        if (errors.Count == 0)
        {
            if (_errors.Remove(propertyName))
            {
                RaiseValidationStateChanged();
                RaiseErrorsChanged(propertyName);
            }

            return;
        }

        _errors[propertyName] = errors;

        RaiseValidationStateChanged();
        RaiseErrorsChanged(propertyName);
    }

    #endregion

    #region Notifications

    /// <summary>
    /// Raises notifications for changes in the validation state. This method is called whenever there is a change in the validation state of the object, such as when validation errors are added, removed, or updated. It raises notifications for the HasErrors property to indicate that the overall error state of the object may have changed, and it also raises notifications for the Errors property to indicate that the collection of validation errors has been updated. By calling this method whenever there are changes to the validation state, you can ensure that any UI elements or other components that are bound to these properties are properly updated to reflect the current state of validation for the object.
    /// </summary>
    private void RaiseValidationStateChanged()
    {
        Owner.NotifyPropertyChanged(nameof(HasErrors));
        Owner.NotifyPropertyChanged(nameof(Errors));
    }

    /// <summary>
    /// Raises the ErrorsChanged event for a specific property. This method is called whenever the validation errors for a property change, and it raises the ErrorsChanged event to notify any subscribers that the validation errors for the specified property have changed. It also raises a notification for the HasErrors property to indicate that the overall error state of the object may have changed. By calling this method whenever validation errors are added, removed, or updated for a property, you can ensure that any UI elements or other components that are bound to the validation errors are properly updated to reflect the current state of validation for that property.
    /// </summary>
    /// <param name="propertyName">The name of the property whose validation errors have changed.</param>
    private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(Owner, new(propertyName));

    #endregion

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        _errors.Clear();

        base.DisposeManagedResources();
    }

    #region Helpers

    /// <summary>
    /// Gets the dependent properties for a given property. This method retrieves the dependent properties for a specified property by looking up the dependencies defined using the AlsoValidateAttribute on the properties of the owner's type. It uses a concurrent dictionary to cache the dependencies for each type, so that subsequent lookups for the same type can be performed efficiently without needing to reflect over the properties again. By calling this method with a property name, you can obtain an array of property names that are dependent on the specified property, allowing you to perform additional validation on those dependent properties when the specified property changes.
    /// </summary>
    /// <param name="propertyName">The name of the property whose dependent properties are to be retrieved.</param>
    /// <returns>An array of property names that are dependent on the specified property.</returns>
    private string[] GetDependents(string propertyName) => MetadataRegistry.Get(Owner.GetType()).GetProperty(propertyName).TryGetFeature<ValidationDependencyFeature>(out var feature)
        ? [.. feature.Dependents]
        : [];

    #endregion
}
