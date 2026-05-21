// -----------------------------------------------------------------------
// <copyright file="LogTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Logging;
using Xunit;

namespace MyNet.Utilities.Tests.Logging;

public class LogTests
{
    [Fact]
    public void Create_ByCategory_UsesConfiguredFactory()
    {
        var factory = new CaptureLoggerFactory();
        var previous = Log.Factory;
        try
        {
            Log.Factory = factory;

            _ = Log.Create("my-category");

            Assert.Contains("my-category", factory.RequestedCategories);
        }
        finally
        {
            Log.Factory = previous;
        }
    }

    [Fact]
    public void Create_Generic_UsesTypeNameCategory()
    {
        var factory = new CaptureLoggerFactory();
        var previous = Log.Factory;
        try
        {
            Log.Factory = factory;

            _ = Log.Create<LogTests>();

            Assert.Contains(typeof(LogTests).FullName!, factory.RequestedCategories);
        }
        finally
        {
            Log.Factory = previous;
        }
    }

    [Fact]
    public void LogT_Instance_UsesConfiguredFactory()
    {
        var factory = new CaptureLoggerFactory();
        var previous = Log.Factory;
        try
        {
            Log.Factory = factory;

            _ = Log<LogTests>.Instance;

            Assert.Contains(typeof(LogTests).FullName!, factory.RequestedCategories);
        }
        finally
        {
            Log.Factory = previous;
        }
    }

    private sealed class CaptureLoggerFactory : ILoggerFactory
    {
        public List<string> RequestedCategories { get; } = [];

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            RequestedCategories.Add(categoryName);
            return new NullLogger();
        }

        public void Dispose()
        {
        }

        private sealed class NullLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
                where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
            }

            private sealed class NullScope : IDisposable
            {
                public static readonly NullScope Instance = new();

                public void Dispose()
                {
                }
            }
        }
    }
}
