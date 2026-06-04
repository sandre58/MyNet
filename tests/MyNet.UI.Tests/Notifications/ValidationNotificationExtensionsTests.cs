// -----------------------------------------------------------------------
// <copyright file="ValidationNotificationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using FluentValidation;
using Moq;
using MyNet.Observable;
using MyNet.Observable.Validation.Validators;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using Xunit;

namespace MyNet.UI.Tests.Notifications;

public sealed class ValidationNotificationExtensionsTests
{
    [Fact]
    public void PublishErrors_PublishesEachError()
    {
        var publisher = new Mock<INotificationPublisher>();

        publisher.Object.PublishErrors(["a", "  ", "b"]);

        publisher.Verify(x => x.Publish(It.Is<MessageNotification>(n => n.Message == "a")), Times.Once);
        publisher.Verify(x => x.Publish(It.Is<MessageNotification>(n => n.Message == "b")), Times.Once);
        publisher.Verify(x => x.Publish(It.IsAny<MessageNotification>()), Times.Exactly(2));
    }

    [Fact]
    public void TryValidateAndNotify_WhenValid_DoesNotPublish()
    {
        var publisher = new Mock<INotificationPublisher>();
        var owner = new ValidOwner();

        owner.TryValidateAndNotify(publisher.Object).Should().BeTrue();

        publisher.Verify(x => x.Publish(It.IsAny<INotification>()), Times.Never);
    }

    [Fact]
    public void TryValidateAndNotify_WhenInvalid_PublishesErrors()
    {
        var publisher = new Mock<INotificationPublisher>();
        var owner = new InvalidOwner();

        owner.TryValidateAndNotify(publisher.Object).Should().BeFalse();

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n => n.Message == "Required" && n.Severity == NotificationSeverity.Error)),
            Times.Once);
    }

    private sealed class ValidOwner : ObservableObject
    {
        public ValidOwner() => this.UseValidation(EmptyValidator.Instance);
    }

    private sealed class InvalidOwner : ObservableObject
    {
        public InvalidOwner() => this.UseValidation(new InvalidOwnerValidator());
    }

    private sealed class InvalidOwnerValidator : AbstractValidator<InvalidOwner>
    {
        public InvalidOwnerValidator() =>
            RuleFor(static x => x).Must(static _ => false).WithMessage("Required");
    }
}
