// -----------------------------------------------------------------------
// <copyright file="ShellNotificationsViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using MyNet.Observable;
using MyNet.Observable.Collections;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Threading;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// View model for the shell notifications panel (list, severity, close actions).
/// Drawer visibility is controlled by <see cref="MainWindowViewModel.IsNotificationsOpen"/>.
/// </summary>
public sealed class ShellNotificationsViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellNotificationsViewModel"/> class.
    /// </summary>
    public ShellNotificationsViewModel(
        INotificationsManager notificationsManager,
        ISchedulerProvider schedulerProvider,
        ICommandFactory? commandFactory = null)
    {
        ArgumentNullException.ThrowIfNull(notificationsManager);
        ArgumentNullException.ThrowIfNull(schedulerProvider);

        var commands = commandFactory ?? RelayCommandFactory.Default;

        Notifications = ExtendedCollection.Create<IClosableNotification>(schedulerProvider.Ui);

        foreach (var notification in notificationsManager.Notifications.OfType<IClosableNotification>())
            Notifications.Add(notification);

        var collection = (INotifyCollectionChanged)notificationsManager.Notifications;
        NotifyCollectionChangedEventHandler collectionChanged = (_, e) => ApplyCollectionChange(e);
        collection.CollectionChanged += collectionChanged;
        Disposables.Add(Disposable.Create(() => collection.CollectionChanged -= collectionChanged));

        ExecuteActionCommand = commands.CreateRequired<ActionNotification>(
            static notification => notification.Action?.Invoke(notification),
            static notification => notification.Action is not null);

        CloseCommand = commands.CreateRequired<IClosableNotification>(static n => n.RequestClose());

        ClearCommand = commands.Create(
            () => Notifications.Where(static x => x.IsClosable).ToList().ForEach(static x => x.RequestClose()),
            () => Notifications.Any(static x => x.IsClosable));

        Disposables.Add(
            Notifications.ToObservableChangeSet().Subscribe(_ =>
            {
                MaxSeverity = Notifications.Count != 0
                    ? Notifications.OfType<MessageNotification>().Max(static x => x.Severity)
                    : null;

                HasNotifications = Notifications.Count > 0;
                NotificationsChanged?.Invoke(this, EventArgs.Empty);
            }));

        RefreshNotificationState();
    }

    /// <summary>
    /// Raised when the notification list or <see cref="HasNotifications"/> changes.
    /// </summary>
    public event EventHandler? NotificationsChanged;

    /// <summary>
    /// Gets the notifications bound to the panel.
    /// </summary>
    public ExtendedCollection<IClosableNotification> Notifications { get; }

    /// <summary>
    /// Gets a value indicating whether at least one notification is present.
    /// </summary>
    public bool HasNotifications { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the highest severity among <see cref="MessageNotification"/> items, if any.
    /// </summary>
    public NotificationSeverity? MaxSeverity { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets the command that executes an actionable notification.
    /// </summary>
    public ICommand ExecuteActionCommand { get; }

    /// <summary>
    /// Gets the command that closes a single notification.
    /// </summary>
    public ICommand CloseCommand { get; }

    /// <summary>
    /// Gets the command that closes all closable notifications.
    /// </summary>
    public ICommand ClearCommand { get; }

    private void ApplyCollectionChange(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var added in e.NewItems!.Cast<INotification>().OfType<IClosableNotification>())
                    Notifications.Add(added);
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var removed in e.OldItems!.Cast<INotification>())
                {
                    var match = Notifications.FirstOrDefault(x => x.Id == removed.Id);
                    if (match is not null)
                        Notifications.Remove(match);
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                Notifications.Clear();
                break;
        }

        RefreshNotificationState();
    }

    private void RefreshNotificationState()
    {
        MaxSeverity = Notifications.Count != 0
            ? Notifications.OfType<MessageNotification>().Max(static x => x.Severity)
            : null;

        HasNotifications = Notifications.Count > 0;
        NotificationsChanged?.Invoke(this, EventArgs.Empty);
    }
}
