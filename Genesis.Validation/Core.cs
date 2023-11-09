namespace Genesis.Validation;

using System.Linq.Expressions;

public static partial class Core {
    /// <summary>
    /// Extension to add an additional rule to an existing rule set
    /// </summary>
    /// <param name="ruleSet">The rule set to add a rule to</param>
    /// <param name="selector">
    /// The selector function which selects the
    /// property to use
    /// </param>
    /// <param name="validator">The validation predicate</param>
    /// <param name="errorMessage">
    /// The optional custom error message to provide when the
    /// validation fails
    /// </param>
    public static IRuleSet<T> RuleFor<T, TReturn>(
        this IRuleSet<T> ruleSet,
        Expression<Func<T, TReturn>> selector,
        Func<TReturn, bool> validator,
        string errorMessage
    ) => new RuleSet<T>(ruleSet.Rules.Append(new Rule<T>(
        selector.Body switch {
            MemberExpression m => m?.Member?.Name ?? "''",
            UnaryExpression u => ((MemberExpression)u.Operand)?.Member?.Name ?? "''",
            ParameterExpression p => p?.Name ?? "''",
            _ => "''"
        },
        x => validator(selector.Compile().Invoke(x)),
        errorMessage
    )));


    /// <summary>
    /// Extension to add an additional rule to an existing rule set
    /// </summary>
    /// <param name="selector">
    /// The selector function which selects the
    /// property to use
    /// </param>
    /// <param name="validator">The validation predicate</param>
    /// <param name="errorMessage">
    /// The optional custom error message to provide when the
    /// validation fails
    /// </param>
    public static IRuleSet<T> RuleFor<T, TReturn>(
        Expression<Func<T, TReturn>> selector,
        Func<TReturn, bool> validator,
        string errorMessage
    ) => new EmptyRuleSet<T>().RuleFor(selector, validator, errorMessage);

    /// <summary>
    /// Extension for validating an actual value
    /// which is validated against an expected rule set
    /// </summary>
    /// </param name="set">The rule set to validate agains</param>
    /// <param name="value">The actual provided value to validate</param>
    public static IRuleSetValidationResult<T> Validate<T>(this IRuleSet<T> set, T value) =>
        new RuleSetValidationResult<T>(set.Rules.Select<IRule<T>, ValidationResult>(
            r => r.Validator(value) switch {
                true  => (ValidationResult) new ValidationResult.Success<T>(value),
                false => new ValidationResult.Failure(
                    r.PropertyName,
                    string.IsNullOrEmpty(r.ErrorMessage)
                        ? $"{r.PropertyName} is invalid"
                        : r.ErrorMessage
                )
            }
        ));
}