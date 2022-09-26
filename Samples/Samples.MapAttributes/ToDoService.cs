namespace Samples.MapAttributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

public enum ToDoState : byte {
    Open, Done
}

record ToDo(int Id, string Title, ToDoState State = ToDoState.Open);

[Route("api")]
sealed class ToDoService {
    //. just here to prove ctor DI works just fine
    //. method DI works as expected with minimal endpoints as well
    readonly ILogger<ToDoService> _logger;

    public ToDoService(ILogger<ToDoService> logger) =>
        _logger = logger;

    readonly List<ToDo> ToDos = new();

    [HttpGet("todos")]
    public IEnumerable<ToDo> GetAllToDos() =>
        ToDos;

    [HttpGet("todos/{id}")]
    public ToDo? GetToDo(int id) =>
        ToDos.FirstOrDefault(t => t.Id == id);

    [HttpPut("todos")]
    public ToDo AddToDo(string message) {
        ToDo todo = new(ToDos.Count, message);
        ToDos.Add(todo);
        return todo;
    }

    [HttpPost("todos/complete")]
    public ToDo? CompleteToDo(int id) =>
        id < ToDos.Count
            ? (ToDos[id] = ToDos[id] with { State = ToDoState.Done })
            : null;
}