namespace Genesis.Validation;

public interface IRuleSet<T> {
    IEnumerable<IRule<T>> Rules { get; }
}