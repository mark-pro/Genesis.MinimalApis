using FluentAssertions;
using Genesis.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Test.Api;

namespace MinimalEndpoints.Tests;

[TestClass]
public class MapEndpointErrorTests {

    WebApplication? _app;

    [TestInitialize]
    public void InitializeTest() =>
        _app = WebApplication.Create();

    [TestMethod]
    public void MapStaticErrorTest() {
        var func = () =>
            _app?.MapStaticEndpoints(typeof(SampleEndpoints));

        func.Should()
            .Throw<ArgumentException>()
            .WithMessage($"{typeof(SampleEndpoints).Name} must be a static class! *")
            .WithParameterName("type");
    }
}