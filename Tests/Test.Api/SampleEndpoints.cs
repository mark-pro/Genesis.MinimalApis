namespace Test.Api;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Genesis.DependencyInjection;
using Genesis.Validation;

[Route("api")]
public sealed class SampleEndpoints : IEndpoints {

    readonly GreetingService _greetingService;

    public SampleEndpoints(GreetingService greetingService) =>
        _greetingService = greetingService;

    public void RegisterEndpoints(IEndpointRouteBuilder app) =>
        app.MapGet("/api/greet", _greetingService.Greet);

    [HttpPatch("echo")]
    [HttpPost("echo")]
    [HttpPut("echo")]
    public string EchoContent([FromBody] string message) =>
        message ?? "";

    [HttpDelete("echo")]
    [HttpGet("echo")]
    public string EchoNonContent([FromQuery] string message) =>
        message ?? "";
}

[Route("api")]
public static class SampleStaticEndpoints {
    [HttpGet("echo/static")]
    [HttpDelete("echo/static")]
    public static string StaticEchoContent([FromQuery] string message) =>
        message ?? "";

    [HttpPatch("echo/static")]
    [HttpPost("echo/static")]
    [HttpPut("echo/static")]
    public static string StaticEchoNonContent([FromBody] string message) =>
        message ?? "";
}

[Route("api/validation")]
public static class ValidationStaticEndpoints {
    [HttpGet("echo-param")]
    [ValidateParam(typeof(EchoRequest))]
    public static string StaticEchoParamContent([AsParameters] EchoRequest request) => request.Message;
    
    [HttpGet("echo-validate")]
    public static string StaticEchoValidateContent([Validate, AsParameters] EchoRequest request) => request.Message;
}