// -----------------------------------------------------------------------
// <copyright file="EncryptionServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using MyNet.Utilities.Encryption;
using Xunit;

namespace MyNet.Utilities.Tests.Encryption;

public class EncryptionServiceTests
{
    private readonly AesEncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        const string key = "g.ul0l6:pc5h?yp?:l1;j70pd;vo4h0.";
        var keys = Encoding.ASCII.GetBytes(key);
        _encryptionService = new(keys);
    }

    [Theory]
    [InlineData("adhlefè'(7483", "PMZjNJRFU1/HDSwu6uwxtQAAAAAAAAAAAAAAAIGeLnjvtyxE6v72r152")]
    [InlineData("Ceci est un test", "Ouy/WaZMTNAnSbcr8g3ezQAAAAAAAAAAAAAAAKOfJX2qtJyY7aOvuxIgB/s=")]
    [InlineData("", "4F1RVR9h31W8xmNnUKRqzwAAAAAAAAAAAAAAAA==")]
    public void Encrypt(string value, string expected)
    {
        var result = _encryptionService.Encrypt(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("PMZjNJRFU1/HDSwu6uwxtQAAAAAAAAAAAAAAAIGeLnjvtyxE6v72r152", "adhlefè'(7483")]
    [InlineData("Ouy/WaZMTNAnSbcr8g3ezQAAAAAAAAAAAAAAAKOfJX2qtJyY7aOvuxIgB/s=", "Ceci est un test")]
    [InlineData("4F1RVR9h31W8xmNnUKRqzwAAAAAAAAAAAAAAAA==", "")]
    public void Decrypt(string value, string expected)
    {
        var result = _encryptionService.Decrypt(value);

        Assert.Equal(expected, result);
    }
}
