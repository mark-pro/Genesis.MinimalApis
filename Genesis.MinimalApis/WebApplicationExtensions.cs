namespace Genesis.DependencyInjection;

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class WebApplicationExtensions {

    /// <summary>
    /// Maps endpoints from functions in a class using <see ref="HttpMethodAttribute" /> and <see ref="RouteAtribute" /> attributes.
    /// <code>
    /// [Route("api")]
    /// public class Foo {
    ///     [HttpGet("echo")]
    ///     public string Echo([FromQuery] string message) =>
    ///         message;    
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T">Type registered in services that contains <see ref="HttpMethodAttribute" /> and <see ref="RouteAtribute" /> attributes.</typeparam>
    public static WebApplication MapEndpoints<T>(this WebApplication app) {
        var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var service = sp.GetService<T>()
            ?? throw new ApplicationException($"Cannot find registered service {typeof(T).FullName}");
        
        (service as IEndpoints)?.RegisterEndpoints(app);
            
        return RegisterAttributes(app, service, typeof(T));
    }

    /// <summary>
    /// Map static class functions using <see ref="HttpMethodAttribute" /> and <see ref="RouteAttribute" /> attributes.
    /// <code>
    /// [Route("api")]
    /// public static class Foo {
    ///     [HttpGet("echo")]
    ///     public static string Echo([FromQuery] string message) =>
    ///         message;    
    /// }
    /// </code>
    /// </summary>
    public static WebApplication MapStaticEndpoints(this WebApplication app, Type type) =>
        type.IsAbstract && type.IsSealed
            ? RegisterAttributes(app, null, type)
            : throw new ArgumentException($"{type.Name} must be a static class!", nameof(type));

    static WebApplication RegisterAttributes(WebApplication app, object? instance, Type type) {
        Delegate CreateDelegate(object? instance, MethodInfo mi) {
            var parameters = 
                mi.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var exprInstance = instance is null ? null : Expression.Constant(instance);

            var call = Expression.Call(exprInstance, mi, parameters);
            
            var lambda = Expression.Lambda(call, parameters).Compile();
            return mi.CreateDelegate(lambda.GetType(), instance);
        }

        string CreateRouteTemplate(string template1, string template2) {
            string ParseRoute (string template) =>
#if NET7_0_OR_GREATER
                template switch {
                    ['/',..] => template[1..],
                    _ => template };
#elif NET6_0_OR_GREATER
                template.StartsWith('/') 
                    ? template[1..] 
                    : template;
#endif
            return $@"{ParseRoute(template1)}/{ParseRoute(template2)}";
        }

        IEnumerable<T> GetCustomAttributes<T>(MethodInfo mi) where T : Attribute =>
            mi.GetCustomAttributes(typeof(T), true).Cast<T>();

        var routeRoot = type.GetCustomAttribute<RouteAttribute>()?.Template ?? "";

        var methods = type.GetMethods()
            .Where(mi => GetCustomAttributes<HttpMethodAttribute>(mi).Any());

        var results = methods
            .SelectMany(mi => 
                GetCustomAttributes<HttpMethodAttribute>(mi)
                .Select(a => (
                    action: CreateDelegate(instance, mi), 
                    attribute: a, 
                    routeTemplate: CreateRouteTemplate(routeRoot, a!.Template!)
                ))
            ).SelectMany(r => r.attribute.HttpMethods.Select(x => (r.action, r.routeTemplate, verb: x)))
            .Where(r => r.verb is "DELETE" or "GET" or "PATCH" or "POST" or "PUT");

        foreach(var (action, routeTemplate, verb) in results)
            (verb switch {
                "DELETE" => (Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder>)
                            EndpointRouteBuilderExtensions.MapDelete,
                "GET"    => EndpointRouteBuilderExtensions.MapGet,
#if NET7_0_OR_GREATER
                "PATCH"  => EndpointRouteBuilderExtensions.MapPatch,
#endif
                "POST"   => EndpointRouteBuilderExtensions.MapPost,
                "PUT"    => EndpointRouteBuilderExtensions.MapPut,
                _ => (_, _, _) => new(Enumerable.Empty<IEndpointConventionBuilder>())
            })(app, routeTemplate, action);

        return app;
    }
}