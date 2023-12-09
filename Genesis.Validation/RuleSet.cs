using Point = (int x, int y);
using FibSequence = int[];

namespace Genesis.Validation;

static class Foo {
    static Point Point => (1, 2);

}

/// <summary>
/// Rule set which contains an enumerable of rules to validate against
/// </summary>
public record struct RuleSet<T>(IEnumerable<IRule<T>> Rules) : IRuleSet<T>;

/// <summary>
/// Represents an empty rule set
/// </summary>
public record EmptyRuleSet<T> : IRuleSet<T> {
    public IEnumerable<IRule<T>> Rules { get; } = Enumerable.Empty<IRule<T>>();
}