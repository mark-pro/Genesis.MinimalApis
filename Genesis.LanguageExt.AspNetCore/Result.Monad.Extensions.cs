namespace Genesis;

using LanguageExt.Common;

public static partial class Core {
    
    /// <summary>
    /// Converts a <seealso cref="Exception" /> to a <seealso cref="IResult" />
    /// </summary>
    /// <returns>An IResult from the Exception provided Exception</returns>
    public readonly static Func<Exception, IResult> Fail =
        compose(ToProblemDetails, Results.Problem);

    /// <summary>
    /// Converts a <seealso cref="Error" /> to a <seealso cref="IResult" />
    /// </summary>
    /// <returns>Returns an IResult from the provided Error</returns>
    public readonly static Func<Error, IResult> Error =
        compose(ToException, Fail);

    /// <summary>
    /// Produces a NotFound <seealso cref="IResult" />
    /// </summary>
    /// <returns>Returns an IResult</returns>
    readonly static Func<IResult> NotFound =
        memo(() => Results.NotFound());

    /// <summary>
    /// Produces either a 200 OK or 204 NoContent <seealso cref="IResult" />
    /// depending on wither the value is Unit or not.
    /// <para>
    /// If the value is Unit, then a 204 NoContent is returned else a 200 OK is returned.
    /// </para>
    /// </summary>
    /// <param name="value">The value to use for the success result</param>
    /// <typeparam name="T">The type binding of the value</typeparam>
    /// <returns>
    /// Either a 200 OK or 204 NoContent <seealso cref="IResult" />
    /// </returns>
    static IResult Success<T>(T value) =>
        value switch {
            Unit => Results.NoContent(),
            _ => Results.Ok(value)
        };

    /// <summary>
    /// Runs the <seealso cref="Try{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="try">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Try{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Try{T}" /> computation
    /// </returns>
    public static IResult ToResult<T>(this Try<T> @try) =>
        @try.Match(Success, Fail);

    /// <summary>
    /// Runs the <seealso cref="TryOption{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="tryOption">
    /// THe computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="TryOption{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="TryOption{T}" /> computation
    /// </returns>
    public static IResult ToResult<T>(this TryOption<T> tryOption) =>
        tryOption.Match(Success, NotFound, Fail);

    /// <summary>
    /// Runs the <seealso cref="Option{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="option">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Option{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Option{T}" /> computation
    /// </returns>
    public static IResult ToResult<T>(this Option<T> option) =>
        option.Match(Success, NotFound);

    /// <summary>
    /// Converts an <seealso cref="Fin{T}" /> to an <seealso cref="IResult" />
    /// <para>
    /// Could return either a 200 OK, 204 NoContent, 404 NotFound, or 500 InternalServerError <seealso cref="IResult" />
    /// </para>
    /// </summary>
    /// <param name="fin">
    /// Converts the provided <seealso cref="Fin{T}" /> to an <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Fin{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Fin{T}" />
    /// </returns>
    public static IResult ToResult<T>(this Fin<Option<T>> fin) =>
        fin.Map(o => o.Map(Success).IfNone(NotFound))
        .IfFail(Error);

    /// <summary>
    /// Converts an <seealso cref="Fin{T}" /> to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="fin">
    /// Converts the provided <seealso cref="Fin{T}" /> to an <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Fin{T}" />
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Fin{T}" />
    /// </returns>
    public static IResult ToResult<T>(this Fin<T> fin) =>
        fin.Match(Success, Error);

    /// <summary>
    /// Run the <seealso cref="Eff{T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="eff">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Eff{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Eff{T}" /> computation
    /// </returns>
    public static IResult ToResult<T>(this Eff<T> eff) =>
        eff.Map(Success).Run().ToResult();

    /// <summary>
    /// Run the <seealso cref="Eff{RT, T}" /> computation and converts the result to an <seealso cref="IResult" />
    /// </summary>
    /// <param name="eff">
    /// The computation to run and convert to a <seealso cref="IResult" />
    /// </param>
    /// <param name="runtime">
    /// The runtime environment to use <seealso cref="Eff{RT, T}" /> for the computation
    /// </param>
    /// <typeparam name="RT">
    /// The type binding of the runtime environment
    /// </typeparam>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Eff{RT, T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IResult" /> from the provided <seealso cref="Eff{RT, T}" /> computation
    /// </returns>
    public static IResult ToResult<RT, T>(this Eff<RT, T> eff, RT runtime) where RT : struct =>
        eff.Map(Success).Run(runtime).ToResult();
}