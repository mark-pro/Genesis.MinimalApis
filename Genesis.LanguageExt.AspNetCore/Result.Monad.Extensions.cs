namespace Genesis;

using LanguageExt.Common;

public static partial class Core {

    public readonly static Func<Exception, IResult> Fail =
        compose(ToProblemDetails, Results.Problem);

    public readonly static Func<Error, IResult> Error =
        compose(ToException, Fail);

    readonly static Func<IResult> NotFound =
        memo(() => Results.NotFound());

    static IResult Success<T>(T value) =>
        value switch {
            Unit => Results.NoContent(),
            _ => Results.Ok(value)
        };

    public static IResult ToResult<T>(this Try<T> @try) =>
        @try.Match(Success, Fail);

    public static IResult ToResult<T>(this TryOption<T> tryOption) =>
        tryOption.Match(Success, NotFound, Fail);

    public static IResult ToResult<T>(this Option<T> option) =>
        option.Match(Success, NotFound);

    public static IResult ToResult<T>(this Fin<Option<T>> fin) =>
        fin.Map(o => o.Map(Success).IfNone(NotFound))
        .IfFail(Error);

    public static IResult ToResult<T>(this Fin<T> fin) =>
        fin.Match(Success, Error);

    public static IResult ToResult<T>(this Eff<T> eff) =>
        eff.Map(Success).Run().ToResult();

    public static IResult ToResult<RT, T>(this Eff<RT, T> eff, RT runtime) where RT : struct =>
        eff.Map(Success).Run(runtime).ToResult();
}