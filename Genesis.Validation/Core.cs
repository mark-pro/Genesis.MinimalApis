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
    /// Start off a rule set with a rule
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
        string errorMessage = ""
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
                true  => new ValidationResult.Success<T>(value),
                false => new ValidationResult.Failure(
                    r.PropertyName,
                    string.IsNullOrEmpty(r.ErrorMessage)
                        ? $"{r.PropertyName} is invalid"
                        : r.ErrorMessage
                )
            }
        ));

    /// <summary>
    /// Extensions to add an additional primitive rule to an existing rule set
    /// </summary>
    /// <typeparam name="T">The primitive type to check</typeparam>
    /// <param name="ruleSet">
    /// The rule set to add a rule to 
    /// </param>
    /// <param name="validator">
    /// The validation predicate
    /// </param>
    /// <param name="errorMessage"></param>
    /// <param name="name">
    /// The optional name of the property to validate
    /// </param>
    /// <returns></returns>
    public static IRuleSet<T> PrimitiveRuleFor<T>(
        this IRuleSet<T> ruleSet,
        Func<T, bool> validator,
        string errorMessage,
        string? name = null
    ) {
        if(!typeof(T).IsPrimitive) 
            throw new ArgumentException("Type must be primitive", typeof(T).Name);
        return new RuleSet<T>(ruleSet.Rules.Append(new Rule<T>(
            name ?? typeof(T).Name,
            validator,
            errorMessage
        )));
    }

    /// <summary>
    /// Start off a rule set with a primitive rule
    /// </summary>
    /// <typeparam name="T">
    /// The primitive type to check
    /// </typeparam>
    /// <param name="validator">
    /// The validation predicate
    /// </param>
    /// <param name="errorMessage">
    /// The optional custom error message to provide when the
    /// </param>
    /// <param name="name">
    /// The optional name of the property to validate
    /// </param>
    /// <returns></returns>
    public static IRuleSet<T> PrimitiveRuleFor<T>(
        Func<T, bool> validator,
        string errorMessage,
        string name = "") =>
        new EmptyRuleSet<T>()
            .PrimitiveRuleFor(validator, errorMessage, name);
}