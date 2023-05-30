using LanguageExt.Effects.Traits;

namespace Genesis;

public static partial class Core {
    public static Task<IActionResult> ToActionResultAsync<T>(this TryAsync<T> handler) =>
        handler.Match(SuccessAction, FailAction);

    public static Task<IActionResult> ToActionResultAsync<T>(this TryOptionAsync<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction, FailAction);

    public static Task<IActionResult> ToActionResultAsync<T>(this OptionAsync<T> handler) =>
        handler.Match(SuccessAction, NotFoundAction);

    public static ValueTask<IActionResult> ToActionResultAsync<T>(this Aff<T> handler) =>
        handler.Map(SuccessAction).Run()
        .Map(r => r.IfFail(ErrorAction));

    public static ValueTask<IActionResult> ToActionResultAsync<RT, T>(this Aff<RT, T> handler, RT runtime) where RT : struct, HasCancel<RT> =>
        handler.Map(SuccessAction).Run(runtime)
        .Map(r => r.IfFail(ErrorAction));
    
}