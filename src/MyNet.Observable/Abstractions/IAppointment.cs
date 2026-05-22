// -----------------------------------------------------------------------
// <copyright file="IAppointment.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Defines a contract for an appointment that supports property change notifications. This interface extends INotifyPropertyChanged, allowing implementing classes to notify subscribers when the StartDate or EndDate properties change. The StartDate and EndDate properties represent the start and end times of the appointment, respectively. By implementing this interface, an appointment can provide a way for clients to react to changes in its scheduling information, which can be useful in applications such as calendar management, scheduling systems, or any scenario where appointments need to be tracked and updated dynamically.
/// </summary>
public interface IAppointment : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the start date and time of the appointment. This property represents when the appointment is scheduled to begin. Implementing classes should raise the PropertyChanged event when this property is set to a new value, allowing subscribers to react to changes in the appointment's start time.
    /// </summary>
    DateTimeOffset StartDate { get; }

    /// <summary>
    /// Gets the end date and time of the appointment. This property represents when the appointment is scheduled to end. Implementing classes should raise the PropertyChanged event when this property is set to a new value, allowing subscribers to react to changes in the appointment's end time.
    /// </summary>
    DateTimeOffset EndDate { get; }
}
