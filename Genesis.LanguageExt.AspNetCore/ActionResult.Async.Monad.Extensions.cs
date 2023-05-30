using LanguageExt.Effects.Traits;

namespace Genesis;

public static partial class Core {

    /// <summary>
    /// Runs the <seealso cref="TryAsync{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to a <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="TryAsync{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="TryAsync{T}" /> computation
    /// </returns>
    public static Task<IActionResult> ToActionResultAsync<T>(this TryAsync<T> handler) =>
        handler.Match(SuccessAction, FailAction);

    /// <summary>
    /// Runs the <seealso cref="TryOptionAsync{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to a <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="TryOptionAsync{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="TryOptionAsync{T}" /> computation
    /// </returns>
    public static Task<IActionResult> ToActionResultAsync<T>(this TryOptionAsync<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction, FailAction);

    /// <summary>
    /// Runs the <seealso cref="OptionAsync{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to a <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="OptionAsync{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="OptionAsync{T}" /> computation
    /// </returns>
    public static Task<IActionResult> ToActionResultAsync<T>(this OptionAsync<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction);

    /// <summary>
    /// Runs the <seealso cref="Aff{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to a <seealso cref="IActionResult" />
    /// </param>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Aff{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Aff{T}" /> computation
    /// </returns>
    public static ValueTask<IActionResult> ToActionResultAsync<T>(this Aff<T> handler) =>
        handler.Map(SuccessAction).Run()
        .Map(r => r.IfFail(ErrorAction));

    /// <summary>
    /// Runs the <seealso cref="Aff{T}" /> computation and converts the result to an <seealso cref="IActionResult" />
    /// </summary>
    /// <param name="handler">
    /// The computation to run and convert to a <seealso cref="IActionResult" />
    /// </param>
    /// <param name="runtime">
    /// The runtime to run the <seealso cref="Aff{T}" /> computation with
    /// </param>
    /// <typeparam name="RT">
    /// The runtime type environment of the <seealso cref="Aff{T}" /> computation
    /// </typeparam>
    /// <typeparam name="T">
    /// The type binding of the <seealso cref="Aff{T}" /> computation
    /// </typeparam>
    /// <returns>
    /// Returns an <seealso cref="IActionResult" /> from the provided <seealso cref="Aff{T}" /> computation
    /// </returns>
    public static ValueTask<IActionResult> ToActionResultAsync<RT, T>(this Aff<RT, T> handler, RT runtime) where RT : struct, HasCancel<RT> =>
        handler.Map(SuccessAction).Run(runtime)
        .Map(r => r.IfFail(ErrorAction));
    
}