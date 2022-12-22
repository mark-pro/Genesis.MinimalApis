using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalEndpoints.Tests;

internal sealed class SampleApp : WebApplicationFactory<Program> {

    private readonly Action<IServiceCollection> _serviceOverride = _ => {};

    public SampleApp(Action<IServiceCollection> serviceOverride) =>
        _serviceOverride = serviceOverride;

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(_serviceOverride);
}