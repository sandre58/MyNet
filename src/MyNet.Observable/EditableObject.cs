// -----------------------------------------------------------------------
// <copyright file="EditableObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MyNet.Observable.Attributes;
using MyNet.Observable.Validation;
using MyNet.Utilities;
using MyNet.Utilities.Suspending;
using PropertyChanged;

namespace MyNet.Observable;

[CanBeValidatedForDeclaredClassOnly(false)]
[CanSetIsModifiedAttributeForDeclaredClassOnly(false)]
public abstract class EditableObject : LocalizableObject, IEditableObject
{
    private readonly PropertyInfo[] _publicProperties;
    private readonly List<INotifyCollectionChanged> _observableCollections;

    // Optimize: Cache for properties that can be validated/modified to avoid repeated filtering
    private PropertyInfo[]? _validatableProperties;
    private PropertyInfo[]? _modifiableProperties;
    private PropertyInfo[]? _notifiableProperties;

    private bool _isModified;

    protected EditableObject()
    {
        _publicProperties = [.. GetType().GetPublicProperties()];
        _observableCollections = GetObservableCollectionsList();

        foreach (var collection in _observableCollections)
        {
            collection.CollectionChanged += CollectionChanged;
        }
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!IsModifiedSuspender.IsSuspended)
            SetIsModified();
    }

    [CanBeValidated(false)]
    [CanSetIsModified(false)]
    public override bool IsDisposed => base.IsDisposed;

    #region Members

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    private Dictionary<string, List<SeverityValidationResult>> ValidationErrors { get; } = [];

    /// <summary>
    /// Gets the rules which provide the errors.
    /// </summary>
    /// <value>The rules this instance must satisfy.</value>
    public ValidationRuleCollection ValidationRules { get; } = [];

    protected ISuspender IsModifiedSuspender { get; set; } = Suspenders.IsModifiedSuspender.Default;

    protected ISuspender ValidatePropertySuspender { get; set; } = Suspenders.ValidatePropertySuspender.Default;

    #endregion Members

    #region INotifyDataErrorInfo

    /// <summary>
    /// Occurs when the validation errors have changed for a property or for the entire object.
    /// </summary>
    protected event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc />
    /// <summary>
    /// Occurs when the validation errors have changed for a property or for the entire object.
    /// </summary>
    event EventHandler<DataErrorsChangedEventArgs>? INotifyDataErrorInfo.ErrorsChanged
    {
        add => ErrorsChanged += value;
        remove => ErrorsChanged -= value;
    }

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "Returns all error for current object")]
    public virtual IEnumerable<string> Errors => GetValidationMessages(ValidationRuleSeverity.Error);

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
    public virtual IEnumerable<string> Warnings => GetValidationMessages(ValidationRuleSeverity.Warning);

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
    public virtual IEnumerable<string> Informations => GetValidationMessages(ValidationRuleSeverity.Information);

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
    public virtual bool HasErrors => ValidationErrors.Values.Any(errors => errors.Any(e => e.Severity == ValidationRuleSeverity.Error));

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
    public virtual bool HasWarnings => ValidationErrors.Values.Any(errors => errors.Any(e => e.Severity == ValidationRuleSeverity.Warning));

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
    public virtual bool HasInformations => ValidationErrors.Values.Any(errors => errors.Any(e => e.Severity == ValidationRuleSeverity.Information));

    // Optimize: Reduce LINQ allocations by iterating once
    private IEnumerable<string> GetValidationMessages(ValidationRuleSeverity severity)
    {
        var messages = new List<string>();
        foreach (var kvp in ValidationErrors)
        {
            foreach (var result in kvp.Value)
            {
                if (result.Severity == severity && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    messages.Add(result.ErrorMessage!);
                }
            }
        }

        return messages.Distinct();
    }

    /// <summary>
    /// Gets the validation errors for a specified property or for the entire object.
    /// </summary>
    /// <param name="propertyName">Name of the property to retrieve errors for. <c>null</c> to
    /// retrieve all errors for this instance.</param>
    /// <returns>A collection of errors.</returns>
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return GetValidationMessages(ValidationRuleSeverity.Error).ToList();
        }

        if (!ValidationErrors.TryGetValue(propertyName, out var errors))
        {
            return Array.Empty<string>();
        }

        // Optimize: Build list once
        var errorMessages = new List<string?>(errors.Count);
        foreach (var error in errors)
        {
            if (error.Severity == ValidationRuleSeverity.Error)
            {
                errorMessages.Add(error.ErrorMessage);
            }
        }

        return errorMessages;
    }

    /// <summary>
    /// Gets the validation errors for the entire object.
    /// </summary>
    /// <returns>A collection of errors.</returns>
    public virtual IEnumerable<string> GetValidationErrors()
    {
        var result = new List<string>((IEnumerable<string>)GetErrors(string.Empty));

        // Optimize: Use cached validatable properties
        var validatableProps = GetValidatableProperties();

        // Complex validation errors
        foreach (var prop in validatableProps)
        {
            if (prop.GetValue(this) is IValidatable entity)
            {
                result.AddRange(entity.GetValidationErrors());
            }
            else if (prop.GetValue(this) is IEnumerable collection && !typeof(IValidatable).IsAssignableFrom(prop.PropertyType))
            {
                foreach (var item in collection)
                {
                    if (item is IValidatable validatable)
                    {
                        result.AddRange(validatable.GetValidationErrors());
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Notifies when errors changed.
    /// </summary>
    [SuppressPropertyChangedWarnings]
    protected void OnErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

    #endregion INotifyDataErrorInfo

    #region IValidatable

    /// <summary>
    /// Test if property is valid.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">The value.</param>
    protected virtual void ValidateProperty(string propertyName, object? value)
    {
        if (ValidatePropertySuspender.IsSuspended) return;

        var prop = GetPropertyByName(propertyName);
        if (prop?.CanBeValidated(this) != true) return;

        var validationResults = new List<SeverityValidationResult>();

        // Validation by Metadata
        var metadataResults = new List<ValidationResult>();
        _ = Validator.TryValidateProperty(value, new ValidationContext(this) { MemberName = propertyName }, metadataResults);

        foreach (var result in metadataResults)
        {
            validationResults.Add(new SeverityValidationResult(result.ErrorMessage, result.MemberNames));
        }

        // Custom Validation
        var customValidations = ValidationRules.Apply(this, propertyName);
        foreach (var validation in customValidations)
        {
            validationResults.Add(new SeverityValidationResult(validation.Error, [propertyName], validation.Severity));
        }

        // Is Valid
        if (validationResults.Count == 0)
        {
            if (ValidationErrors.Remove(propertyName))
            {
                RaiseValidation();
                OnErrorsChanged(propertyName);
            }

            return;
        }

        // Is not valid
        ValidationErrors[propertyName] = validationResults;
        RaiseValidation();
        OnErrorsChanged(propertyName);
    }

    public virtual bool ValidateProperties()
    {
        if (ValidatePropertySuspender.IsSuspended) return true;

        var result = true;
        var properties = GetValidatableProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(this);

            // Validate property
            ValidateProperty(property.Name, value);
            result = result && !HasErrors;

            // Complex property
            if (value is IValidatable entity)
            {
                result = result && entity.ValidateProperties();
            }

            // Collection property
            else if (value is ICollection collection)
            {
                foreach (var item in collection)
                {
                    if (item is IValidatable validatable)
                    {
                        result = result && validatable.ValidateProperties();
                    }
                }
            }
        }

        return result;
    }

    public void ResetValidation()
    {
        var propertiesNotValid = ValidationErrors.Keys.ToArray();
        ValidationErrors.Clear();
        RaiseValidation();

        foreach (var propertyName in propertiesNotValid)
        {
            OnErrorsChanged(propertyName);
        }
    }

    private void RaiseValidation()
    {
        OnPropertyChanged(nameof(Errors));
        OnPropertyChanged(nameof(Warnings));
        OnPropertyChanged(nameof(Informations));
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(HasWarnings));
        OnPropertyChanged(nameof(HasInformations));
    }

    #endregion IValidatable

    #region IPropertyChanged

    /// <summary>
    /// Called when [property changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="before">The before value.</param>
    /// <param name="after">The after value.</param>
    protected virtual void OnPropertyChanged(string propertyName, object before, object after)
    {
        if (PropertyChangedSuspender.IsSuspended || GetType().GetCustomAttributes<CanNotifyAttribute>().Any(x => !x.Value)) return;

        var prop = GetPropertyByName(propertyName);
        if (prop?.CanNotify() != true) return;

        // Validation
        if (prop.CanBeValidated(this))
        {
            ValidateProperty(propertyName, after);

            // Validate other property defined by [ValidateProperty(<PropertyName>)] attributes.
            var validateAttributes = prop.GetCustomAttributes<ValidatePropertyAttribute>();
            foreach (var attr in validateAttributes)
            {
                var property = GetPropertyByName(attr.PropertyName);
                if (property != null)
                {
                    var value = property.GetValue(this);
                    ValidateProperty(attr.PropertyName, value);
                }
            }
        }

        // Modification
        if (!IsModifiedSuspender.IsSuspended && prop.CanSetIsModified(this))
        {
            SetIsModified();
            OnPropertyIsModified(propertyName, before, after);
        }

        // Notification
        OnPropertyChanged(propertyName);
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Used by Fody")]
    protected virtual void OnPropertyIsModified(string propertyName, object before, object after) { }

    #endregion IPropertyChanged

    #region IModifiable

    /// <inheritdoc />
    /// <summary>
    /// Gets a value indicates if the entity has been modified.
    /// </summary>
    public virtual bool IsModified()
    {
        if (_isModified) return true;

        var propertiesToCheck = GetModifiableProperties();

        // Check complex properties
        foreach (var prop in propertiesToCheck)
        {
            var value = prop.GetValue(this);

            if (value is IModifiable modifiable && modifiable.IsModified())
            {
                return true;
            }

            if (value is ICollection collection && !typeof(IModifiable).IsAssignableFrom(prop.PropertyType))
            {
                foreach (var item in collection)
                {
                    if (item is IModifiable itemModifiable && itemModifiable.IsModified())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <inheritdoc />
    /// <summary>
    /// Reset IsModified value.
    /// </summary>
    public virtual void ResetIsModified()
    {
        _isModified = false;
        var propertiesToCheck = GetModifiableProperties();

        // Reset complex properties
        foreach (var prop in propertiesToCheck)
        {
            var value = prop.GetValue(this);

            if (value is IModifiable modifiable)
            {
                modifiable.ResetIsModified();
            }
            else if (value is ICollection collection && !typeof(IModifiable).IsAssignableFrom(prop.PropertyType))
            {
                foreach (var item in collection)
                {
                    if (item is IModifiable itemModifiable)
                    {
                        itemModifiable.ResetIsModified();
                    }
                }
            }
        }
    }

    protected virtual void SetIsModified() => _isModified = true;

    #endregion IModifiable

    #region Helper Methods - Optimize property lookups

    /// <summary>
    /// Gets a property by name with caching.
    /// </summary>
    private PropertyInfo? GetPropertyByName(string propertyName) => Array.Find(_publicProperties, p => p.Name == propertyName);

    /// <summary>
    /// Gets cached validatable properties.
    /// </summary>
    private PropertyInfo[] GetValidatableProperties()
    {
        if (_validatableProperties != null)
            return _validatableProperties;

        var props = new List<PropertyInfo>(_publicProperties.Length);
        foreach (var prop in _publicProperties)
        {
            if (prop.CanBeValidated(this))
            {
                props.Add(prop);
            }
        }

        _validatableProperties = [.. props];
        return _validatableProperties;
    }

    /// <summary>
    /// Gets cached modifiable properties.
    /// </summary>
    private PropertyInfo[] GetModifiableProperties()
    {
        if (_modifiableProperties != null)
            return _modifiableProperties;

        var props = new List<PropertyInfo>(_publicProperties.Length);
        foreach (var prop in _publicProperties)
        {
            if (prop.CanSetIsModified(this))
            {
                props.Add(prop);
            }
        }

        _modifiableProperties = [.. props];
        return _modifiableProperties;
    }

    /// <summary>
    /// Gets observable collections that need to be monitored.
    /// </summary>
    private List<INotifyCollectionChanged> GetObservableCollectionsList()
    {
        var collections = new List<INotifyCollectionChanged>();

        foreach (var prop in _publicProperties)
        {
            if (prop.CanSetIsModified() && typeof(ICollection).IsAssignableFrom(prop.PropertyType))
            {
                if (prop.GetValue(this) is INotifyCollectionChanged collection)
                {
                    collections.Add(collection);
                }
            }
        }

        return collections;
    }

    #endregion

    protected override void Cleanup()
    {
        foreach (var collection in _observableCollections)
        {
            collection.CollectionChanged -= CollectionChanged;
        }

        // Clear caches
        _validatableProperties = null;
        _modifiableProperties = null;
        _notifiableProperties = null;

        base.Cleanup();
    }
}
