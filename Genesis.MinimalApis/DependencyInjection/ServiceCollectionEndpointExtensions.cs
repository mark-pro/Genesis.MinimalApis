namespace Genesis.DependencyInjection;

public static class ServiceCollectionEndpointExtensions {
    /// <summary>
    /// Allows for the register of structs that implement <seealso cref="IEndpoints"/>.
    /// </summary>
    public static IServiceCollection RegisterEndpoints<T>(this IServiceCollection services, Func<IServiceProvider, object> func) where T : IEndpoints =>
        services.AddSingleton(typeof(T), func);

    /// <summary>
    /// Adds a singleton instance of <seealso cref="IEndpoints"/> to the service collection.
    /// </summary>
    public static IServiceCollection AddSingletonEndpoints<T>(this IServiceCollection services) where T : class, IEndpoints =>
        services.AddSingleton<T>();

    /// <summary>
    /// Adds a scoped instance of <seealso cref="IEndpoints"/> to the service collection.
    /// </summary>
    public static IServiceCollection AddScopedEndpoints<T>(this IServiceCollection services) where T : class, IEndpoints =>
        services.AddScoped<T>();
    
    /// <summary>
    /// Adds a transient instance of <seealso cref="IEndpoints"/> to the service collection.
    /// </summary>
    public static IServiceCollection AddTransientEndpoints<T>(this IServiceCollection services) where T : class, IEndpoints =>
        services.AddTransient<T>();
}