namespace Samples.MapAttributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Genesis.Validation;
using FluentValidation;

public enum ToDoState : byte {
    Open, Done
}

record ToDo(int Id, string Title, ToDoState State = ToDoState.Open);

class Message : ValidatableType<Message, string> {
    public Message(string message) : base(message) {}
}

class MessageValidator : AbstractValidator<Message> {
    public MessageValidator() {
        RuleFor(x => (string) x).NotNull().NotEmpty().Matches("^[a-zA-Z]+$");
    }
}

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

    [HttpPut("todos/create")]
    [Validate(typeof(Message))]
    public ToDo AddToDo(Message message) {
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