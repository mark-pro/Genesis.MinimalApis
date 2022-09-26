namespace Genesis.DependencyInjection;

using Microsoft.AspNetCore.Routing;

public interface IEndpoints {
    /// <summary>
    /// Mutates the IEndpointRouteBuilder to include
    /// endpoints in the registered function.
    /// </summary>
    /// <param name="app">Essentially the 
    /// <see ref="Microsoft.AspNetCore.Builder.WebApplication" /> to mutate</param>
    void RegisterEndpoints(IEndpointRouteBuilder app);
}
