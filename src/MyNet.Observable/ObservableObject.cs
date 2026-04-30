// -----------------------------------------------------------------------
// <copyright file="ObservableObject.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using MyNet.Utilities;
using MyNet.Utilities.Suspending;

namespace MyNet.Observable;

/// <summary>
/// A base class for objects of which the properties must be observable.
/// </summary>
public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
    protected CompositeDisposable Disposables { get; } = [];

    protected ISuspender PropertyChangedSuspender { get; } = Suspenders.PropertyChangedSuspender.Default;

    public virtual bool IsDisposed => _disposedValue;

    #region INotifyPropertyChanged

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => PropertyChangedHandler += value;
        remove => PropertyChangedHandler -= value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Implement interface")]
    private event PropertyChangedEventHandler? PropertyChangedHandler;

    /// <summary>
    /// Raises the PropertyChanged event if needed.
    /// </summary>
    /// <remarks>If the propertyName parameter
    /// does not correspond to an existing property on the current class, an
    /// exception is thrown in DEBUG configuration only.</remarks>
    /// <param name="propertyName">The name of the property that
    /// changed.</param>
    protected void OnPropertyChanged(string? propertyName)
    {
        if (PropertyChangedSuspender.IsSuspended) return;

        PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raises the PropertyChanged event if needed.
    /// </summary>
    /// <typeparam name="T">The type of the property that
    /// changed.</typeparam>
    /// <param name="propertyExpression">An expression identifying the property
    /// that changed.</param>
    protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
        var handler = PropertyChangedHandler;

        if (handler == null)
            return;

        var propertyName = propertyExpression.GetPropertyName();

        if (!string.IsNullOrEmpty(propertyName))
        {
            OnPropertyChanged(propertyName);
        }
    }

    #endregion

    #region INotifyPropertyChanging

    public event PropertyChangingEventHandler? PropertyChanging
    {
        add => PropertyChangingHandler += value;
        remove => PropertyChangingHandler -= value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "Implement interface")]
    private event PropertyChangingEventHandler? PropertyChangingHandler;

    /// <summary>
    /// Raises the PropertyChanging event if needed.
    /// </summary>
    /// <remarks>If the propertyName parameter
    /// does not correspond to an existing property on the current class, an
    /// exception is thrown in DEBUG configuration only.</remarks>
    /// <param name="propertyName">The name of the property that
    /// changed.</param>
    protected void OnPropertyChanging(string propertyName)
    {
        if (PropertyChangedSuspender.IsSuspended) return;

        PropertyChangingHandler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    /// <summary>
    /// Raises the PropertyChanging event if needed.
    /// </summary>
    /// <typeparam name="T">The type of the property that
    /// changes.</typeparam>
    /// <param name="propertyExpression">An expression identifying the property
    /// that changes.</param>
    protected void OnPropertyChanging<T>(Expression<Func<T>> propertyExpression)
    {
        if (PropertyChangedSuspender.IsSuspended) return;

        var propertyName = propertyExpression.GetPropertyName();
        PropertyChangingHandler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    #endregion

    #region IDisposable Support

    private bool _disposedValue;

    protected virtual void Cleanup() => Disposables.Dispose();

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (!disposing)
            Cleanup();

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support
}
