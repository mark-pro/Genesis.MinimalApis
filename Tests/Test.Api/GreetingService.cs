namespace Test.Api;

public sealed class GreetingService {
    public string Greet(string name) =>
        $"Hello {name}!";
}