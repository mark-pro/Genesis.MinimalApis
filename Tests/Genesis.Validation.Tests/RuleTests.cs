namespace Genesis.Validation.Tests;

using Xunit.Sdk;
using static Genesis.Validation.Core;

record Message(string Content);

public class RuleSetTests {
    [Fact]
    public void RuleFor_Success() {
        var ruleSet = 
            RuleFor<Message, string>(
                x => x.Content,
                x => x.Length > 5,
                "Message must be longer than 5 characters"
        );

        var result = ruleSet.Validate(new Message("Hello World"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [MemberData(nameof(PrimitiveRuleValues))]
    public void PrimitiveRuleFor_Tests(int value, bool expected) {
        var ruleSet = PrimitiveRuleFor(
            (int x) => x > 5,
            "Value must be greater than 5"
        ).PrimitiveRuleFor(
            x => x < 10,
            "Value must be less than 10"
        );

        var result = ruleSet.Validate(value);

        result.IsValid.Should().Be(expected);

        if (!result.IsValid)
            result.Validations.Where(x => x is ValidationResult.Failure)
                .Should().HaveCount(1);
    }

    public static IEnumerable<object[]> PrimitiveRuleValues() {
        yield return new object[] { 4, false };
        yield return new object[] { 10, false };
        yield return new object[] { 6, true };
        yield return new object[] { 9, true };
    }

    [Fact]
    public void PrimitiveRuleFor_Non_Primitive_Type() {
        try {
            var ruleSet = PrimitiveRuleFor(
                (Message x) => x.Content.Length > 5,
                "Value must be greater than 5"
            );
        } catch (ArgumentException e) {
            e.ParamName.Should().Be("Message");
            return;
        }

        Assert.Fail("Should have thrown ArgumentException");
    }
}