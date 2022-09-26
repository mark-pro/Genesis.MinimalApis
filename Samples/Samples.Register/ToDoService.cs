namespace Samples.Register;

using Microsoft.AspNetCore.Routing;
using Genesis.DependencyInjection;

public enum ToDoState : byte {
    Open, Done
}

record ToDo(int Id, string Title, ToDoState State = ToDoState.Open);

sealed class ToDoService : IEndpoints {
    //. just here to prove ctor DI works just fine
    //. method DI works as expected with minimal endpoints as well
    readonly ILogger<ToDoService> _logger;

    public ToDoService(ILogger<ToDoService> logger) =>
        _logger = logger;

    readonly List<ToDo> ToDos = new();

    public IEnumerable<ToDo> GetAllToDos() =>
        ToDos;

    public ToDo? GetToDo(int id) =>
        ToDos.FirstOrDefault(t => t.Id == id);

    public ToDo AddToDo(string message) {
        ToDo todo = new(ToDos.Count, message);
        ToDos.Add(todo);
        return todo;
    }

    public ToDo? CompleteToDo(int id) =>
        id < ToDos.Count
            ? (ToDos[id] = ToDos[id] with { State = ToDoState.Done })
            : null;

    public void RegisterEndpoints(IEndpointRouteBuilder app) {
        app.MapGet("api/todos/{id}", GetToDo);

        app.MapGet("api/todos", GetAllToDos);

        app.MapPut("api/todos", AddToDo);
        
        app.MapPost("api/todos/complete", CompleteToDo);
    }
}