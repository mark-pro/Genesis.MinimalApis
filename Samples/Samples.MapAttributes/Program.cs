using Genesis.DependencyInjection;
using Samples.MapAttributes;
using global::FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options => 
        options.MapType<Message>(() => new() { Type = "string" })
);

//. register endpoints here as singleton (or other lifetime)
builder.Services.AddSingleton<ToDoService>();
builder.Services.AddScoped<IValidator<Message>, MessageValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//. Map attributes to minimal api routes
app.MapEndpoints<ToDoService>();

app.Run();