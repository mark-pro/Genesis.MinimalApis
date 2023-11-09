namespace Genesis.Validation;

/// <summary>
/// Rule set which containes an enumerable of rules to validate againsts
/// </summary>
public record struct RuleSet<T>(IEnumerable<IRule<T>> Rules) : IRuleSet<T>;

/// <summary>
/// Represents an empty rule set
/// </summary>
public record EmptyRuleSet<T> : IRuleSet<T> {
    public IEnumerable<IRule<T>> Rules { get; } = Enumerable.Empty<IRule<T>>();
}