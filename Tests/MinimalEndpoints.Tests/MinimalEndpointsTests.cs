namespace MinimalEndpoints.Tests;

using Genesis.DependencyInjection;
using FluentAssertions;
using Test.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;


[TestClass]
public sealed class MinimalEndpointsTests {

    enum Register {
        Transient, Scoped, Singleton, Default, Static
    }

    enum HttpVerb {
        Get, Post, Patch, Put, Delete
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

    static async Task DITest(Register register) {
        await using var app = CreateSampleApp(register);
        using var s = app.Services.CreateScope();
        s.ServiceProvider.GetService<SampleEndpoints>()
            .Should().NotBeNull();
    }

    static async Task EndpointRegisterTest(Register register) {
        await using var app = CreateSampleApp(register);
        var client = app.CreateClient();
        var response = await client.GetStringAsync("api/greet?name=sample");
        response.Should().Be("Hello sample!");
    }

    static async Task EndpointAttributeMapTest(Register register, HttpVerb verb) {
        await using var app = CreateSampleApp(register);
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

        var json = JsonContent.Create("hello");

        var response = await (contentFunc?.Invoke("api/echo", json) 
            ?? nonContentFunc.Invoke("api/echo?message=hello"));

        (await response.Content.ReadAsStringAsync())
            .Should().Be("hello");
    }

    [TestMethod]
    public async Task SingletonRegisterTest() =>
        await DITest(Register.Singleton);
    
    [TestMethod]
    public async Task ScopedRegisterTest() =>
        await DITest(Register.Scoped);
    
    [TestMethod]
    public async Task TransientRegisterTest() =>
        await DITest(Register.Transient);

    [TestMethod]
    public async Task RegisterTest() =>
        await DITest(Register.Default);

    [TestMethod]
    public async Task TransientEndpointRegisterTest() =>
        await EndpointRegisterTest(Register.Transient);
    
    [TestMethod]
    public async Task ScopedEndpointRegisterTest() =>
        await EndpointRegisterTest(Register.Scoped);
    
    [TestMethod]
    public async Task SingletonEndpointRegisterTest() =>
        await EndpointRegisterTest(Register.Singleton);

    [TestMethod]
    public async Task DefaultEndpointRegisterTest() =>
        await EndpointRegisterTest(Register.Default);

    [TestMethod]
    public async Task TransientEndpointGetAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Transient, HttpVerb.Get);
    
    [TestMethod]
    public async Task ScopedEndpointGetAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Scoped, HttpVerb.Get);
    
    [TestMethod]
    public async Task SingletonEndpointGetAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Singleton, HttpVerb.Get);
    
    [TestMethod]
    public async Task DefaultEndpointGetAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Default, HttpVerb.Get);

    [TestMethod]
    public async Task StaticEndpointGetAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Static, HttpVerb.Get);

    [TestMethod]
    public async Task TransientEndpointPostAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Transient, HttpVerb.Post);
    
    [TestMethod]
    public async Task ScopedEndpointPostAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Scoped, HttpVerb.Post);
    
    [TestMethod]
    public async Task SingletonEndpointPostAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Singleton, HttpVerb.Post);
    
    [TestMethod]
    public async Task DefaultEndpointPostAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Default, HttpVerb.Post);

    [TestMethod]
    public async Task StaticEndpointPostAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Static, HttpVerb.Post);
    
#if NET7_0_OR_GREATER
    [TestMethod]
    public async Task TransientEndpointPatchAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Transient, HttpVerb.Patch);

    [TestMethod]
    public async Task ScopedEndpointPatchAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Scoped, HttpVerb.Patch);

    [TestMethod]
    public async Task SingletonEndpointPatchAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Singleton, HttpVerb.Patch);
    
    [TestMethod]
    public async Task DefaultEndpointPatchAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Default, HttpVerb.Patch);

    [TestMethod]
    public async Task StaticEndpointPatchAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Static, HttpVerb.Patch);
#endif

    [TestMethod]
    public async Task TransientEndpointPutAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Transient, HttpVerb.Put);
    
    [TestMethod]
    public async Task ScopedEndpointPutAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Scoped, HttpVerb.Put);
    
    [TestMethod]
    public async Task SingletonEndpointPutAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Singleton, HttpVerb.Put);
    
    [TestMethod]
    public async Task DefaultEndpointPutAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Default, HttpVerb.Put);

    [TestMethod]
    public async Task StaticEndpointPutAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Static, HttpVerb.Put);
    
    [TestMethod]
    public async Task TransientEndpointDeleteAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Transient, HttpVerb.Delete);
    
    [TestMethod]
    public async Task ScopedEndpointDeleteAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Scoped, HttpVerb.Delete);
    
    [TestMethod]
    public async Task SingletonEndpointDeleteAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Singleton, HttpVerb.Delete);

    [TestMethod]
    public async Task DefaultEndpointDeleteAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Default, HttpVerb.Delete);
        
    [TestMethod]
    public async Task StaticEndpointDeleteAttrMapTest() =>
        await EndpointAttributeMapTest(Register.Static, HttpVerb.Delete);
}