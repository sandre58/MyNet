// -----------------------------------------------------------------------
// <copyright file="ValidationBehaviorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentValidation;
using MyNet.Metadata;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public void ValidateExtension_ReturnsFalse_WhenNoValidationBehaviorIsRegistered()
    {
        var sut = new ValidationOwner();

        Assert.False(sut.Validate());
    }

    [Fact]
    public void ValidationBehavior_ValidatesObjectAndTracksErrors()
    {
        MetadataRegistry.For<ValidationOwner>()
            .Property(x => x.Name)
            .Validates(nameof(ValidationOwner.Confirm));

        var sut = new ValidationOwner();
        var behavior = sut.UseValidation(new ValidationOwnerValidator());

        sut.Name = string.Empty;
        sut.Confirm = "x";

        Assert.False(behavior.Validate());
        Assert.True(behavior.HasErrors);
        Assert.Contains(behavior.Errors, x => x.Contains("required", StringComparison.OrdinalIgnoreCase)
            || x.Contains("obligatoire", StringComparison.OrdinalIgnoreCase));

        sut.Name = "abc";
        sut.Confirm = "abc";

        Assert.True(sut.Validate());
        Assert.False(behavior.HasErrors);
    }

    private sealed class ValidationOwner : ObservableObject
    {
        public string Name
        {
            get;
            set
            {
                var before = field;
                if (!OnPropertyChanging(nameof(Name), before, value))
                    return;
                field = value;
                OnPropertyChanged(nameof(Name), before, value);
            }
        }

            = string.Empty;

        public string Confirm
        {
            get;
            set
            {
                var before = field;
                if (!OnPropertyChanging(nameof(Confirm), before, value))
                    return;
                field = value;
                OnPropertyChanged(nameof(Confirm), before, value);
            }
        }

            = string.Empty;
    }

    private sealed class ValidationOwnerValidator : AbstractValidator<ValidationOwner>
    {
        public ValidationOwnerValidator()
        {
            this.RuleForLocalized(x => x.Name).NotEmptyRequired();
            RuleFor(x => x.Confirm).Equal(x => x.Name).WithMessage("Confirm must match Name");
        }
    }
}
