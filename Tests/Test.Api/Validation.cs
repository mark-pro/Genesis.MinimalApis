namespace Test.Api;

using FluentValidation;

public record EchoRequest(string Message);

class EchoRequestValidator : AbstractValidator<EchoRequest> {
    public EchoRequestValidator() =>
        RuleFor(er => er.Message).NotNull();
}