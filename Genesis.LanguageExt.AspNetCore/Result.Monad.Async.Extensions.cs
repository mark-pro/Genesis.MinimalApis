using LanguageExt.Effects.Traits;

namespace Genesis;

public static partial class Core {
    
    /// <summary>
    /// Runs the <seealso cref="TryAsync{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="tryAsync">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="TryAsync{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="TryAsync{T}" /> computation
    /// </returns>
    public static Task<IResult> ToResultAsync<T>(this TryAsync<T> tryAsync) =>
        tryAsync.Match(Success, Fail);

    /// <summary>
    /// Runs the <seealso cref="TryOption{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="tryOptionAsync">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="TryOption{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="TryOption{T}" /> computation
    /// </returns>
    public static Task<IResult> ToResultAsync<T>(this TryOptionAsync<T> tryOptionAsync) =>
        tryOptionAsync.Match(Success, NotFound, Fail);

    /// <summary>
    /// Runs the <seealso cref="OptionAsync{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="optionAsync">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="OptionAsync{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="OptionAsync{T}" /> computation
    /// </returns>
    public static Task<IResult> ToResultAsync<T>(this OptionAsync<T> optionAsync) =>
        optionAsync.Match(Success, NotFound);

    /// <summary>
    /// Runs the <seealso cref="Aff{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="aff">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Aff{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Aff{T}" /> computation
    /// </returns>
    public static ValueTask<IResult> ToResultAsync<T>(this Aff<T> aff) =>
        aff.Map(Success)
        .Run()
        .Map(r => r.ToResult());

    /// <summary>
    /// Runs the <seealso cref="Aff{RT, T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="aff">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <param name="runtime">
    /// The runtime to use to run the <seealso cref="Aff{RT, T}" /> computation
    /// </param>
    /// <typeparam name="RT">
    /// The runtime type environment binding of the <seealso cref="Aff{RT, T}" /> computation
    /// </typeparam>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Aff{RT, T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Aff{RT, T}" /> computation
    /// </returns>
    public static ValueTask<IResult> ToResultAsync<RT, T>(this Aff<RT, T> aff, RT runtime) where RT : struct, HasCancel<RT> =>
        aff.Map(Success)
        .Run(runtime)
        .Map(r => r.ToResult());
}