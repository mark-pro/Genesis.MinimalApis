namespace Genesis.DependencyInjection;

using System.Linq.Expressions;
using System.Reflection;
using Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Routing;

public static class WebApplicationExtensions {

    const string _DELETE = "DELETE";
    const string _GET = "GET";
    const string _PATCH = "PATCH";
    const string _POST = "POST";
    const string _PUT = "PUT";
    
    delegate RouteHandlerBuilder HttpVerbFunctor(IEndpointRouteBuilder routeBuilder, string routeTemplate, Delegate action);
    
    /// <summary>
    /// Maps endpoints from functions in a class using <see ref="HttpMethodAttribute" /> and <see ref="RouteAttribute" /> attributes.
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
        type is { IsAbstract: true , IsSealed: true }
            ? RegisterAttributes(app, null, type)
            : throw new ArgumentException($"{type.Name} must be a static class!", nameof(type));

    static WebApplication RegisterAttributes(WebApplication app, object? instance, Type type) {

        string CreateRouteTemplate(string template1, string template2) {
            string ParseRoute (string template) =>
                template switch {
                    ['/',..] => template[1..],
                    _ => template };
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
                    httpMethod: a,
                    filters: GetCustomAttributes<ValidateParamAttribute>(mi)
                        .Concat(
                            mi.GetParameters()
                            .Where(p => p.GetCustomAttribute<ValidateAttribute>() is not null)
                            .Select(p => new ValidateParamAttribute(p.ParameterType))
                        ).DistinctBy(attr => attr.Type),
                    routeTemplate: CreateRouteTemplate(routeRoot, a.Template!)
                ))
            ).SelectMany(r => r.httpMethod.HttpMethods.Select(x => (r.action, r.routeTemplate, verb: x, r.filters)))
            .Where(r => r.verb is _DELETE or _GET or _PATCH or _POST or _PUT);

        foreach(var (action, routeTemplate, verb, filters) in results) {
            HttpVerbFunctor mapDelete = EndpointRouteBuilderExtensions.MapDelete;
            HttpVerbFunctor mapGet = EndpointRouteBuilderExtensions.MapGet;
            HttpVerbFunctor mapPatch = EndpointRouteBuilderExtensions.MapPatch;
            HttpVerbFunctor mapPost = EndpointRouteBuilderExtensions.MapPost;
            HttpVerbFunctor mapPut = EndpointRouteBuilderExtensions.MapPut;
            
            var routeBuilder = (verb switch {
                _DELETE => mapDelete,
                _GET    => mapGet,
                _PATCH  => mapPatch,
                _POST   => mapPost,
                _PUT    => mapPut,
                _ => (_, _, _) => new(Enumerable.Empty<IEndpointConventionBuilder>())
            })(app, routeTemplate, action);
            
            foreach(var filter in filters)
                typeof(RouteHandlerBuilderExtensions)
                    .GetMethod("AddValidationFilter", 1, new[] { typeof(RouteHandlerBuilder) })
                    ?.MakeGenericMethod(filter.Type)
                    .Invoke(null, new object[] { routeBuilder });
        }
        return app;

        Delegate CreateDelegate(object? inst, MethodInfo mi) {
            var parameters = 
                mi.GetParameters()
                    .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                    .ToArray();

            var exprInstance = inst is null ? null : Expression.Constant(inst);

            var call = Expression.Call(exprInstance, mi, parameters.Cast<Expression>());
            
            var lambda = Expression.Lambda(call, parameters).Compile();
            return mi.CreateDelegate(lambda.GetType(), inst);
        }
    }
}