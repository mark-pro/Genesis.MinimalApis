namespace Genesis.DependencyInjection; 

public static class ServiceCollectionValidationExtensions {
    /// <summary>
    /// Adds a validator to the service collection.
    /// </summary>
    /// <typeparam name="TValidator">
    /// The type of the validator to add to the service collection.
    /// </typeparam>
    /// <param name="services">
    /// The service collection to add the validator to.
    /// </param>
    /// <param name="serviceLifetime">
    /// The DI lifetime of the validator. Defaults to <seealso cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <returns>
    /// Returns the service collection with the validator added.
    /// </returns>
    public static IServiceCollection AddValidator<TValidator>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TValidator : class, IValidator =>
        AddIValidator(serviceLifetime)(services, typeof(TValidator));

    /// <summary>
    /// Adds all public validators from the assembly containing the type provided.
    /// </summary>
    /// <typeparam name="T">
    /// The type to use for assembly scanning.
    /// </typeparam>
    /// <param name="services">
    /// The service collection to add the validators to.
    /// </param>
    /// <param name="serviceLifetime">
    /// The DI lifetime of the validators. Defaults to <seealso cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <returns>
    /// Returns the service collection with the validators added.
    /// </returns>
    public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where T : class =>
        AddValidatorsFromAssemblyContaining<T>(services, null, serviceLifetime);

    /// <summary>
    /// Adds all public validators from the assembly containing the type provided.
    /// </summary>
    /// <typeparam name="T">
    /// The type to use for assembly scanning.
    /// </typeparam>
    /// <param name="services">
    /// The service collection to add the validators to.
    /// </param>
    /// <param name="filter">
    /// The filter to use to determine which validators to add.
    /// </param>
    /// <param name="serviceLifetime">
    /// The DI lifetime of the validators. Defaults to <seealso cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <returns>
    /// Returns the service collection with the validators added.
    /// </returns>
    public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services , Func<IValidator , bool>? filter , ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where T : class =>
        typeof(T).Assembly.GetTypes()
        .Filter(x => x.IsAssignableTo(typeof(IValidator)) && !x.IsAbstract &&
            Optional(filter).Map(f => Activator.CreateInstance(x) as IValidator is var v && f(v!)).IfNone(true))
        .Fold(services, AddIValidator(serviceLifetime));

    private static Option<Type> GetGenericType(Option<Type> type) =>
        type.Bind(t => Optional(t.GetGenericArguments().FirstOrDefault()));

    private static Type CreateIValidator(Type type) => 
        typeof(IValidator<>).MakeGenericType(type);

    private static Func<IServiceCollection, Type, IServiceCollection> AddIValidator(ServiceLifetime serviceLife) =>
        (services, type) =>
            GetGenericType(type.BaseType)
            .Map(CreateIValidator)
            .Fold(services, (serviceCollection, t) => {
                serviceCollection.Add(new (t, type, serviceLife));
                return serviceCollection;
            });
}