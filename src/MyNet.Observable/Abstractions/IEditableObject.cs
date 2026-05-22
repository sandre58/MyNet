// -----------------------------------------------------------------------
// <copyright file="IEditableObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for editable objects that support property change notifications, validation awareness, and modification tracking. This interface combines the functionality of IObservableObject, IValidationAware, and IModificationAware, allowing implementing classes to notify subscribers when a property is about to change and after it has changed, while also providing mechanisms for validating property values and tracking modifications. This is particularly useful in scenarios where the editable object needs to ensure that its state is valid before allowing changes to be committed, and where it needs to track whether any modifications have been made since the last commit or since the object was created. By implementing this interface, an object can provide a robust editing experience with built-in validation and modification tracking capabilities, which can be beneficial in applications that involve user input or data manipulation.
/// </summary>
public interface IEditableObject : IObservableObject, IValidationAware, IModificationAware;
