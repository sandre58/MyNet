// -----------------------------------------------------------------------
// <copyright file="DisplayTextStrategyRegistryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Humanizer.Display;
using Xunit;

namespace MyNet.Humanizer.Tests.Display;

public class DisplayTextStrategyRegistryTests
{
    [Fact]
    public void Resolver_UsesExactTypeBeforeFallbacks()
    {
        var resolver = BuildResolver(services =>
        {
            services.AddDisplayTextStrategy<ConcreteType, ExactProvider>();
            services.AddDisplayTextStrategy<IContract, InterfaceProvider>();
            services.AddDisplayTextStrategy<BaseType, BaseProvider>();
        });

        var provider = resolver.GetRequired<ConcreteType>();
        var result = provider.GetDisplayText(new(), DisplayTextOptions.Default, CultureInfo.InvariantCulture);

        Assert.Equal("exact", result);
    }

    [Fact]
    public void Resolver_UsesInterfaceBeforeBaseType()
    {
        var resolver = BuildResolver(services =>
        {
            services.AddDisplayTextStrategy<IContract, InterfaceProvider>();
            services.AddDisplayTextStrategy<BaseType, BaseProvider>();
        });

        var provider = resolver.GetRequired<ConcreteType>();
        var result = provider.GetDisplayText(new(), DisplayTextOptions.Default, CultureInfo.InvariantCulture);

        Assert.Equal("interface", result);
    }

    [Fact]
    public void Resolver_UsesBaseTypeWhenNoInterfaceProviderExists()
    {
        var resolver = BuildResolver(services => services.AddDisplayTextStrategy<BaseType, BaseProvider>());
        var provider = resolver.GetRequired<ConcreteType>();
        var result = provider.GetDisplayText(new(), DisplayTextOptions.Default, CultureInfo.InvariantCulture);

        Assert.Equal("base", result);
    }

    [Fact]
    public void Resolver_UsesObjectFallbackWhenNothingElseMatches()
    {
        var resolver = BuildResolver(services => services.AddDisplayTextStrategy<object, ObjectProvider>());
        var provider = resolver.GetRequired<ConcreteType>();
        var result = provider.GetDisplayText(new(), DisplayTextOptions.Default, CultureInfo.InvariantCulture);

        Assert.Equal("object", result);
    }

    [Fact]
    public void Resolver_AdaptsEnumProviderToSpecificEnumType()
    {
        var resolver = BuildResolver(services => services.AddDisplayTextStrategy<Enum, EnumProvider>());
        var provider = resolver.GetRequired<TestEnum>();
        var result = provider.GetDisplayText(TestEnum.Pending, DisplayTextOptions.Default, CultureInfo.InvariantCulture);

        Assert.Equal("enum", result);
    }

    private static IDisplayTextStrategyResolver BuildResolver(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
        return services.BuildServiceProvider().GetRequiredService<IDisplayTextStrategyResolver>();
    }

    private interface IContract;

    private class BaseType : IContract;

    private sealed class ConcreteType : BaseType;

    private enum TestEnum
    {
        Pending
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a display name provider in DI tests.")]
    private sealed class ExactProvider : IDisplayTextStrategy<ConcreteType>
    {
        public string GetDisplayText(ConcreteType value, DisplayTextOptions options, CultureInfo culture) => "exact";

        public string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => "exact";
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a display name provider in DI tests.")]
    private sealed class InterfaceProvider : IDisplayTextStrategy<IContract>
    {
        public string GetDisplayText(IContract value, DisplayTextOptions options, CultureInfo culture) => "interface";

        public string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => "interface";
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a display name provider in DI tests.")]
    private sealed class BaseProvider : IDisplayTextStrategy<BaseType>
    {
        public string GetDisplayText(BaseType value, DisplayTextOptions options, CultureInfo culture) => "base";

        public string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => "base";
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a display name provider in DI tests.")]
    private sealed class ObjectProvider : IDisplayTextStrategy<object>
    {
        public string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => "object";
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a display name provider in DI tests.")]
    private sealed class EnumProvider : IDisplayTextStrategy<Enum>
    {
        public string GetDisplayText(Enum value, DisplayTextOptions options, CultureInfo culture) => "enum";

        public string GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => "enum";
    }
}
