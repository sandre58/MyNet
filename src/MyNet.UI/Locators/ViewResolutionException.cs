// -----------------------------------------------------------------------
// <copyright file="ViewResolutionException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators;

/// <summary>
/// Thrown when a view cannot be resolved from a view model type or instantiated.
/// </summary>
public sealed class ViewResolutionException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewResolutionException"/> class.
    /// </summary>
    public ViewResolutionException()
        : this("View resolution failed.", null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewResolutionException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ViewResolutionException(string message)
        : this(message, null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewResolutionException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ViewResolutionException(string message, Exception innerException)
        : this(message, null, null, innerException)
    {
    }

    /// <summary>
    /// Gets the view model type involved in the failure, when applicable.
    /// </summary>
    public Type? ViewModelType { get; }

    /// <summary>
    /// Gets the view type involved in the failure, when applicable.
    /// </summary>
    public Type? ViewType { get; }

    private ViewResolutionException(string message, Type? viewModelType, Type? viewType, Exception? innerException)
        : base(message, innerException)
    {
        ViewModelType = viewModelType;
        ViewType = viewType;
    }

    /// <summary>
    /// Creates an exception when no view type mapping exists for a view model.
    /// </summary>
    public static ViewResolutionException NoMapping(Type viewModelType) =>
        new(
            $"No view type mapping found for view model '{viewModelType.FullName}'. " +
            "Register a manual mapping via ITypeResolver.Register or add a naming convention.",
            viewModelType,
            null,
            null);

    /// <summary>
    /// Creates an exception when a view type cannot be instantiated.
    /// </summary>
    public static ViewResolutionException CannotInstantiateView(Type viewType, Exception? innerException = null) =>
        new(
            $"Cannot create an instance of view '{viewType.FullName}'. " +
            "Register the view in DI or provide a public parameterless constructor.",
            null,
            viewType,
            innerException);

    /// <summary>
    /// Creates an exception when the resolved view instance is not assignable to the expected type.
    /// </summary>
    public static ViewResolutionException ResolvedViewTypeMismatch(Type viewModelType, Type resolvedViewType, Type expectedViewType) =>
        new(
            $"View resolved for '{viewModelType.FullName}' is '{resolvedViewType.FullName}', " +
            $"but '{expectedViewType.FullName}' was expected.",
            viewModelType,
            resolvedViewType,
            null);
}
