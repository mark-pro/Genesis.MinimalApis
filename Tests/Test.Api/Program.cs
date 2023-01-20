using Genesis.DependencyInjection;
using Test.Api;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IValidator<EchoRequest>, EchoRequestValidator>();
builder.Services.AddSingleton<GreetingService>();
builder.Services.AddSingleton<SampleEndpoints>();

var app = builder.Build();

app.MapEndpoints<SampleEndpoints>();
app.MapStaticEndpoints(typeof(SampleStaticEndpoints));
app.MapStaticEndpoints(typeof(ValidationStaticEndpoints));
app.Run();

public partial class Program {}