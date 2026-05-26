// -----------------------------------------------------------------------
// <copyright file="EmailTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Mail.Models;
using Xunit;

namespace MyNet.Mail.Tests;

public class EmailTests
{
    [Fact]
    public void From_SetsSenderAddress()
    {
        var email = Email.From("sender@test.com", "Sender");

        Assert.Equal("sender@test.com", email.Data.From.Address);
        Assert.Equal("Sender", email.Data.From.Name);
    }

    [Fact]
    public void To_WithSemicolonSeparatedAddresses_SplitsRecipients()
    {
        var email = Email.From("from@test.com")
            .To("a@test.com;b@test.com", "Alice;Bob");

        Assert.Equal(2, email.Data.To.Count);
        Assert.Equal("a@test.com", email.Data.To[0].Address);
        Assert.Equal("Alice", email.Data.To[0].Name);
        Assert.Equal("b@test.com", email.Data.To[1].Address);
        Assert.Equal("Bob", email.Data.To[1].Name);
    }

    [Fact]
    public void To_WithEmptyAddress_IsIgnored()
    {
        var email = Email.From("from@test.com").To("  ");

        Assert.Empty(email.Data.To);
    }

    [Fact]
    public void Cc_Bcc_ReplyTo_ArePopulated()
    {
        var email = Email.From("from@test.com")
            .Cc("cc@test.com")
            .Bcc("bcc@test.com")
            .ReplyTo("reply@test.com");

        Assert.Single(email.Data.Cc);
        Assert.Single(email.Data.Bcc);
        Assert.Single(email.Data.ReplyTo);
    }

    [Fact]
    public void Subject_Body_AndPriority_AreApplied()
    {
        var email = Email.From("from@test.com")
            .Subject("Hello")
            .Body("<p>Hi</p>", isHtml: true)
            .PlaintextAlternativeBody("Hi")
            .HighPriority();

        Assert.Equal("Hello", email.Data.Subject);
        Assert.Equal("<p>Hi</p>", email.Data.Body);
        Assert.True(email.Data.IsHtml);
        Assert.Equal("Hi", email.Data.PlaintextAlternativeBody);
        Assert.Equal(Priority.High, email.Data.Priority);
    }

    [Fact]
    public void LowPriority_SetsLowPriority()
    {
        var email = Email.From("from@test.com").LowPriority();

        Assert.Equal(Priority.Low, email.Data.Priority);
    }

    [Fact]
    public void Attach_DoesNotAddDuplicateAttachment()
    {
        var attachment = new Attachment { Filename = "doc.pdf" };
        var email = Email.From("from@test.com")
            .Attach(attachment)
            .Attach(attachment);

        Assert.Single(email.Data.Attachments);
    }

    [Fact]
    public void AttachFromFilename_WithEmptyFilename_Throws()
    {
        var email = Email.From("from@test.com");

        Assert.Throws<ArgumentException>(() => email.AttachFromFilename(""));
    }

    [Fact]
    public void Header_WithEmptyName_IsIgnored()
    {
        var email = Email.From("from@test.com").Header(" ", "value").Header("X-Test", "1");

        Assert.Single(email.Data.Headers);
        Assert.Equal("1", email.Data.Headers["X-Test"]);
    }

    [Fact]
    public void ToString_IncludesFromToAndSubject()
    {
        var text = Email.From("from@test.com")
            .To("to@test.com")
            .Subject("Subject")
            .ToString();

        Assert.Contains("from@test.com", text, StringComparison.Ordinal);
        Assert.Contains("to@test.com", text, StringComparison.Ordinal);
        Assert.Contains("Subject", text, StringComparison.Ordinal);
    }
}
