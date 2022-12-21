namespace Genesis.DependencyInjection;

public static class ServiceExtensions {
    /// <summary>
    /// Allows for the register of structs that implement <see ref="IEndpoints" />.
    /// </summary>
    public static IServiceCollection RegisterEndpoints<T>(this IServiceCollection services, Func<IServiceProvider, object> func) where T : IEndpoints =>
        services.AddSingleton(typeof(T), func);

    public static IServiceCollection AddSingletonEndpoints<T>(this IServiceCollection services) where T : class, IEndpoints =>
        services.AddSingleton<T>();

    public static IServiceCollection AddScopedEndpoints<T>(this IServiceCollection services) where T : class, IEndpoints =>
        services.AddScoped<T>();

    public static IServiceCollection AddTransientEndpoints<T>(this IServiceCollection services) where T : class, IEndpoints =>
        services.AddTransient<T>();
}