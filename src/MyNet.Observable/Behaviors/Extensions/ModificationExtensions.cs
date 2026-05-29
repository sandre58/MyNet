// -----------------------------------------------------------------------
// <copyright file="ModificationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Behaviors;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ModificationExtensions
{
    extension(ObservableObject owner)
    {
        /// <summary>
        /// Determines whether the observable object has been modified since it was last marked as clean. This method checks if the object implements the IModificationTrackingBehavior interface and returns its IsModified property. If the object does not implement IModificationTrackingBehavior, it returns false, indicating that the object is not considered modified.
        /// </summary>
        /// <returns>True if the object has been modified; otherwise, false.</returns>
        public bool IsModified()
        {
            if (owner.Behaviors.TryGet<ModificationTrackingBehavior>(out var tracking))
                tracking.EnsureInitialStateAttached();

            return owner is IModificationAware { IsModified: true }
                || (owner.Behaviors.TryGet<IModificationTrackingBehavior>(out var behavior) && behavior.IsModified);
        }

        /// <summary>
        /// Resets the modification state when a <see cref="ModificationTrackingBehavior"/> is registered.
        /// </summary>
        public void ResetIsModified()
        {
            if (owner.Behaviors.TryGet<IModificationTrackingBehavior>(out var behavior))
                behavior.ResetModified();
            else if (owner is IModificationAware modificationAware)
                modificationAware.ResetModified();
        }
    }
}
