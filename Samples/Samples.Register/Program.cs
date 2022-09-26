using Genesis.DependencyInjection;
using Samples.Register;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//. Add ToDo service as singleton
builder.Services.AddSingletonEndpoints<ToDoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//. use the endpoints registered in ToDo service
app.MapEndpoints<ToDoService>();

app.Run();