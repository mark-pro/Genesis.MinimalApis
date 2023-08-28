namespace Genesis.DependencyInjection;

using System.Linq.Expressions;
using System.Reflection;
using Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Routing;

public static class WebApplicationExtensions {

    private delegate RouteHandlerBuilder HttpFunctor(IEndpointRouteBuilder builder, string routeTemplate, Delegate action);

    private const string _DELETE = "DELETE";
    private const string _GET = "GET";
    private const string _PATCH = "PATCH";
    private const string _POST = "POST";
    private const string _PUT = "PUT";
    
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

    private static WebApplication RegisterAttributes(WebApplication app, object? instance, Type type) {
        Delegate CreateDelegate(object? objInstance, MethodInfo mi) {
            var parameters = 
                mi.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var exprInstance = objInstance is null ? null : Expression.Constant(objInstance);

            var call = Expression.Call(exprInstance, mi, parameters.Cast<Expression>());
            
            var lambda = Expression.Lambda(call, parameters).Compile();
            return mi.CreateDelegate(lambda.GetType(), objInstance);
        }

        string CreateRouteTemplate(string template1, string template2) {
            string ParseRoute (string template) =>
                template switch {
                    ['/',..] => template[1..],
                    _ => template };
            return $"{ParseRoute(template1)}/{ParseRoute(template2)}";
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
                            .Where(parameterInfo => parameterInfo.GetCustomAttribute<ValidateAttribute>() is not null)
                            .Select(parameterInfo => new ValidateParamAttribute(parameterInfo.ParameterType))
                        ).DistinctBy(validateParamAttribute => validateParamAttribute.Type),
                    routeTemplate: CreateRouteTemplate(routeRoot, a.Template!)
                ))
            ).SelectMany(result => result.httpMethod.HttpMethods.Select(httpMethod => (result.action, result.routeTemplate, verb: httpMethod, result.filters)))
            .Where(result => result.verb is _DELETE or _GET or _PATCH or _POST or _PUT);

        HttpFunctor mapDelete = EndpointRouteBuilderExtensions.MapDelete;
        HttpFunctor mapGet = EndpointRouteBuilderExtensions.MapGet;
        HttpFunctor mapPatch = EndpointRouteBuilderExtensions.MapPatch;
        HttpFunctor mapPost = EndpointRouteBuilderExtensions.MapPost;
        HttpFunctor mapPut = EndpointRouteBuilderExtensions.MapPut;
        
        foreach(var (action, routeTemplate, verb, filters) in results) {
            var routeBuilder = (verb switch {
                _DELETE => mapDelete,
                _GET    => mapGet,
                _PATCH  => mapPatch,
                _POST   => mapPost,
                _PUT    => mapPut,
                _ => (_, _, _) => new RouteHandlerBuilder(Enumerable.Empty<IEndpointConventionBuilder>())
            })(app, routeTemplate, action);
            
            foreach(var filter in filters)
                typeof(RouteHandlerBuilderExtensions)
                    .GetMethod("AddValidationFilter", 1, new[] { typeof(RouteHandlerBuilder) })
                    ?.MakeGenericMethod(filter.Type)
                    .Invoke(null, new object[] { routeBuilder });
        }
        return app;
    }
}