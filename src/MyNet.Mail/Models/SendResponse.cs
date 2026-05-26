// -----------------------------------------------------------------------
// <copyright file="SendResponse.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Mail.Models;

public class SendResponse
{
    public string MessageId { get; set; } = string.Empty;

    public IList<string> ErrorMessages { get; } = [];

    public bool Successful => ErrorMessages.Count == 0;
}
