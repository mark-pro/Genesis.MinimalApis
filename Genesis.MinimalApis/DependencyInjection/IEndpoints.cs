namespace Genesis.DependencyInjection;

public interface IEndpoints {
    /// <summary>
    /// Mutates the IEndpointRouteBuilder to include
    /// endpoints in the registered function.
    /// </summary>
    /// <param name="app">Essentially the 
    /// <seealso ref="Microsoft.AspNetCore.Builder.WebApplication" /> to mutate</param>
    void RegisterEndpoints(IEndpointRouteBuilder app);
}
