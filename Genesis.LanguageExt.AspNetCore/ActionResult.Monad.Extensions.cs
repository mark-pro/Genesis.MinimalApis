using LanguageExt.Common;

namespace Genesis;

public static partial class Core {

    /// <summary>
    /// Converts and <seealso cref="Exception" /> to an <seealso cref="IActionResult" />
    /// </summary>
    /// <returns></returns>
    public readonly static Func<Exception, IActionResult> FailAction =
        compose(
            ToProblemDetails, 
            pd =>
                new ObjectResult(pd) {
                    StatusCode = pd.Status
                }
        );
    
    /// <summary>
    /// Converts an <seealso cref="Error" /> to an <seealso cref="IActionResult" />
    /// </summary>
    public readonly static Func<Error, IActionResult> ErrorAction =
        compose(ToException, FailAction);

    /// <summary>
    /// Produces an <seealso cref="IActionResult" />
    /// <para>
    /// If the value is Unit, then a 204 NoContent is returned else a 200 OK is returned with the provided value.
    /// </para>
    /// </summary>
    /// <param name="value">
    /// The value to use for the success result
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the value
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> of either a 200 OK with the value as the body or 204 NoContent if the value is Unit
    /// </returns>
    static IActionResult SuccessAction<T>(T value) =>
        value switch {
            Unit => new NoContentResult(),
            _ => new OkObjectResult(value)
        };

    /// <summary>
    /// Produces a NotFound <seealso cref="IActionResult" />
    /// </summary>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> of NotFound
    /// </returns>
    readonly static Func<IActionResult> NotFoundAction =
        memo(() => new NotFoundResult());

    /// <summary>
    /// Runs the <seealso cref="Try{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Try{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Try{T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<T>(this Try<T> handler) =>
        handler.Match(SuccessAction, FailAction);

    /// <summary>
    /// Runs the <seealso cref="TryOption{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="TryOption{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="TryOption{T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<T>(this TryOption<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction, FailAction);
    
    /// <summary>
    /// Converts an <seealso cref="Option{T}" /> to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The <seealso cref="Option{T}" /> to convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Option{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Option{T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<T>(this Option<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction);

    /// <summary>
    /// Converts an <seealso cref="Fin{T}" /> to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The <seealso cref="Fin{T}" /> to convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Fin{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Fin{T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<T>(this Fin<Option<T>> handler) =>
        handler.Map(o => o.Match(SuccessAction, NotFoundAction))
        .IfFail(ErrorAction);

    /// <summary>
    /// Converts an <seealso cref="Fin{T}" /> to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The <seealso cref="Fin{T}" /> to convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Fin{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Fin{T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<T>(this Fin<T> handler) =>
        handler.Match(SuccessAction, ErrorAction);

    /// <summary>
    /// Runs the <seealso cref="Eff{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Eff{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Eff{T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<T>(this Eff<T> handler) =>
        handler.Map(SuccessAction).Run().IfFail(ErrorAction);

    /// <summary>
    /// Runs the <seealso cref="Eff{RT, T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to an <seealso cref="IActionResult" />
    /// </param>
    /// <param name="runtime">
    /// The runtime to use for the <seealso cref="Eff{RT, T}" /> computation
    /// </param>
    /// <typeparam name="RT">
    /// The runtime type environment type binding of the <seealso cref="Eff{RT, T}" /> computation
    /// </typeparam>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Eff{RT, T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Eff{RT, T}" /> computation
    /// </returns>
    public static IActionResult ToActionResult<RT, T>(this Eff<RT, T> handler, RT runtime) where RT : struct =>
        handler.Map(SuccessAction).Run(runtime).IfFail(ErrorAction);

}