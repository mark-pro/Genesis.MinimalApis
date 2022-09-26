using Genesis.DependencyInjection;
using Test.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GreetingService>();
builder.Services.AddSingleton<SampleEndpoints>();

var app = builder.Build();

app.MapEndpoints<SampleEndpoints>();
app.MapStaticEndpoints(typeof(SampleStaticEndpoints));
app.Run();

public partial class Program {}