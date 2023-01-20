namespace MinimalEndpoints.Tests;

using Genesis.DependencyInjection;
using Test.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;

[TestClass]
public sealed class MinimalEndpointsTests {

    public enum Register {
        Transient, Scoped, Singleton, Default, Static
    }

    public enum HttpVerb {
        Get, Patch, Post, Put, Delete
    }

    static Action<IServiceCollection> RegisterAction<T>(Register register, Func<IServiceProvider, object>? func = null) where T : class, IEndpoints =>
        register switch {
            Register.Transient => (IServiceCollection services) => services.AddTransientEndpoints<T>(),
            Register.Scoped => services => services.AddScopedEndpoints<T>(),
            Register.Singleton => services => services.AddSingletonEndpoints<T>(),
            Register.Default => services => services.RegisterEndpoints<SampleEndpoints>(func ?? (_ => new())),
            _ => _ => {}
        };

    static SampleApp CreateSampleApp(Register register) =>
        new (RegisterAction<SampleEndpoints>(
            register: register, 
            func: sp => new SampleEndpoints(sp.GetService<GreetingService>()!))
        );

    [TestMethod]
    [DynamicData(nameof(RegisterData))]
    public async Task DITest(Register register) {
        await using var app = CreateSampleApp(register);
        using var s = app.Services.CreateScope();
        s.ServiceProvider.GetService<SampleEndpoints>()
            .Should().NotBeNull();
    }

    [TestMethod]
    [DynamicData(nameof(RegisterData))]
    public async Task EndpointRegisterTest(Register register) {
        await using var app = CreateSampleApp(register);
        var response = await app.CreateClient()
            .GetStringAsync("api/greet?name=sample");

        response.Should().Be("Hello sample!");
    }

    [TestMethod]
    [DynamicData(nameof(MapAttributeData), DynamicDataSourceType.Method)]
    public async Task EndpointAttributeMapTest(Register register, HttpVerb verb) {
        using var app = CreateSampleApp(register);
        var client = app.CreateClient();
        
        Func<string?, HttpContent?, Task<HttpResponseMessage>>? contentFunc = 
            verb switch {
                HttpVerb.Post => client.PostAsync,
                HttpVerb.Put => client.PutAsync,
                HttpVerb.Patch => client.PatchAsync,
                _ => null
            };

        Func<string?, Task<HttpResponseMessage>> nonContentFunc =
            verb switch {
                HttpVerb.Get => client.GetAsync,
                HttpVerb.Delete => client.DeleteAsync,
                _ => _ => Task.FromResult(new HttpResponseMessage())
            };

        var content = "hello";

        var json = JsonContent.Create(content);

        var response = await (contentFunc?.Invoke("api/echo", json) 
            ?? nonContentFunc.Invoke($"api/echo?message={content}"));

        (await response.Content.ReadAsStringAsync())
            .Should().Be(content);
    }

    [TestMethod]
    [DataRow("validate")]
    [DataRow("param")]
    public async Task EndpointValidationAttributeTest(string endpoint) {
        using var app = new SampleApp(sp => {});
        var client = app.CreateClient();
        var uri = $"api/validation/echo-{endpoint}";

        var request1 = client.GetAsync(uri);
        var request2 = client.GetAsync($"{uri}?message=hello");
        (await request1).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await request2).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    static IEnumerable<object[]> MapAttributeData() =>
        Enum.GetValues<Register>()
            .SelectMany(r => 
                Enum.GetValues<HttpVerb>()
                    .Select(v => new object[] { r, v })
            );
            
    static IEnumerable<object[]> RegisterData =>
        Enum.GetValues<Register>()
            .Except(new[] { Register.Static })
            .Select(r => new object[] { r });
}