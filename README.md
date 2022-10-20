# Minimal Endpoints Dependency Injection

[![.NET](https://github.com/mark-pro/Genesis.MinimalApis/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mark-pro/Genesis.MinimalApis/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/mark-pro/Genesis.MinimalApis/actions/workflows/codeql-analysis.yml/badge.svg?branch=master)](https://github.com/mark-pro/Genesis.MinimalApis/actions/workflows/codeql-analysis.yml)
[![nuget](https://img.shields.io/nuget/v/Genesis.MinimalApis)](https://www.nuget.org/packages/Genesis.MinimalApis/)
[![license](https://img.shields.io/github/license/mark-pro/Genesis.MinimalApis)](LICENSE)

Create a minimal endpoint from any function in any file.

Genesis.MinimalApis provides support for easily exposing minimal endpoints from any file.

> __Important!__
> 
> _`MapPatch` and `HttpPatchAttribute` are only supported in dotnet 7.0 and greater! Use `MapPost` and `HttpPostAttribute` in dotnet 6.0!_ 
> 
> _Please reference [MapPatch](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.endpointroutebuilderextensions.mappatch?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-6.0)_

## Getting Started

Add the following package from [nuget](https://www.nuget.org/packages/Genesis.MinimalApis)

`dotnet add package Genesis.MinimalApis`

## Usage

Any function in pretty much any file can be exposed as a minimal endpoint via adding the `IEndpoints` interface to a class or struct or by adding any `HttpMethodAttribute` found in `Microsoft.AspNetCore.Mvc`.

Full DI support is provided for endpoints registered either manually or by the use of attributes. Additional examples are provided under _Samples_.

Endpoints can be registered under any lifecycle.
- Transient
- Scoped
- Singleton

If registering a class that inherits from `IEndpoints` helper dependency injection functions for services are provided.
- `AddTransientEndpoints<T>(this IServiceCollection services) where T : IEndpoint`
- `AddScopedEndpoints<T>(this IServiceCollection services) where T : IEndpoint`
- `AddSingletonEndpoints<T>(this IServiceCollection services) where T : IEndpoint`

Structs that inherit from `IEndpoints` may also be registered, though they require a but more setup.

- `RegisterEndpoints<T>(this IServiceCollection services, Func<ServiceProvider, object> func);`

Static classes can be registered when using attributes found in `Microsoft.AspNetCore.Mvc`.

### Manual Definition

Endpoints can be defined and used in class files by using the `IEndpoints` interface. 

```csharp
//. GreetingService.cs
internal sealed class GreetingService : IEndpoints {
    //. function to expose as an endpoint
    public string Greet(string name) =>
        $"Hello {name}!";

    //. function from interface to which will be used to register the app.
    public void RegisterEndpoints(IEndpointRouteBuilder app) {
        app.MapGet("greet/{name}", Greet)
    };
}

//. Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingletonEndpoints<GreetingService>();

var app = builder.Build();

//. Register endpoint GET /greet/{name} in GreetingService.cs
app.MapEndpoints<GreetingService>();

app.Run();
```

### Attribute Definition

Endpoints can be exposed by using attributes found in `Microsoft.AspNetCore.Mvc` and `Microsoft.AspNetCore.Mvc.Routing`.

Supported Attributes:
- [`HttpGet`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpgetattribute?view=aspnetcore-6.0)
- [`HttpPost`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httppostattribute?view=aspnetcore-6.0)
- [`HttpPatch`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httppatchattribute?view=aspnetcore-6.0)
- [`HttpPut`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpputattribute?view=aspnetcore-6.0)
- [`HttpDelete`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.httpdeleteattribute?view=aspnetcore-6.0)

```csharp
//. GreetingService.cs
using Microsoft.AspNetCore.Mvc;

[Route("api")]
internal sealed class GreetingService {
    [HttpGet("greet/{name}")]
    public string Greet(string name) =>
        $"Hello {name}!";
}

//. Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GreetingService>();

var app = builder.Build();

//. Map attributes in GreetingService to GET /api/Greet/{name}
app.MapEndpoints<GreetingService>();

app.Run();
```

### Static Attribute Definition

Attributes can be used on static classes such that static functions can be registered as endpoint.

_Only method injection will work using this method of registration as static classes cannot be instantiated._

```csharp
//. GreetingFunctions.cs
using Microsoft.AspNetCore.Mvc;

[Route("api")]
internal static class GreetingFunctions {
    [HttpGet("greet/{name}")]
    public static string Greet(string name) =>
        $"Hello {name}!";
}

//. Program.cs
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

//. Map attributes in GreetingFunctions to GET /api/Greet/{name}
app.MapStaticEndpoints<GreetingService>();

app.Run();
```

