using LanguageExt.Common;

namespace Genesis;

public static partial class Core {
    public readonly static Func<Exception, IActionResult> FailAction =
        compose(
            ToProblemDetails, 
            pd =>
                new ObjectResult(pd) {
                    StatusCode = pd.Status
                }
        );
    
    public readonly static Func<Error, IActionResult> ErrorAction =
        compose(ToException, FailAction);

    static IActionResult SuccessAction<T>(T value) =>
        value switch {
            Unit => new NoContentResult(),
            _ => new OkObjectResult(value)
        };

    readonly static Func<IActionResult> NotFoundAction =
        memo(() => new NotFoundResult());

    public static IActionResult ToActionResult<T>(this Try<T> handler) =>
        handler.Match(SuccessAction, FailAction);

    public static IActionResult ToActionResult<T>(this TryOption<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction, FailAction);
    
    public static IActionResult ToActionResult<T>(this Option<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction);

    public static IActionResult ToActionResult<T>(this Fin<Option<T>> handler) =>
        handler.Map(o => o.Match(SuccessAction, NotFoundAction))
        .IfFail(ErrorAction);

    public static IActionResult ToActionResult<T>(this Fin<T> handler) =>
        handler.Match(SuccessAction, ErrorAction);

    public static IActionResult ToActionResult<T>(this Eff<T> handler) =>
        handler.Map(SuccessAction).Run().IfFail(ErrorAction);

    public static IActionResult ToActionResult<RT, T>(this Eff<RT, T> handler, RT runtime) where RT : struct =>
        handler.Map(SuccessAction).Run(runtime).IfFail(ErrorAction);

}