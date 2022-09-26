using Genesis.DependencyInjection;
using Samples.MapAttributes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//. register endpoints here as singleton (or other lifetime)
builder.Services.AddSingleton<ToDoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//. Map attributes to minimal api routes
app.MapEndpoints<ToDoService>();

app.Run();