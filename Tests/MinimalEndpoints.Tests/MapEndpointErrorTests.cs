using FluentAssertions;
using Genesis.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Test.Api;

namespace MinimalEndpoints.Tests;

[TestClass]
public class MapEndpointErrorTests : IAsyncDisposable {

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

    [TestCleanup]
    public Task CleanupTest() =>
        DisposeAsync().AsTask();

    public ValueTask DisposeAsync() =>
        _app?.DisposeAsync() ?? ValueTask.CompletedTask;
}