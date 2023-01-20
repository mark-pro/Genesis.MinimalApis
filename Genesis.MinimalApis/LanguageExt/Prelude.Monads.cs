using LanguageExt.Common;

namespace Genesis;

public static partial class Prelude {

    static IResult Fail(Exception e) =>
        Results.Problem(e.ToProblemDetails());

    static Func<Error, IResult> FailWithError =>
        compose(
            (Error e) => e.Exception
                .IfNone(new Exception(e.Message))
                .ToProblemDetails(),
            Results.Problem
        );

    static IResult Ok<T>(T value) =>
        Results.Ok(value);

    static IResult NotFound() =>
        Results.NotFound();

    static IResult NoContent(Unit _) =>
        Results.NoContent();

    public static IResult ToResult<T>(this Try<T> @try) =>
        @try.Match(Ok<T>, Fail);

    public static IResult ToResult<T>(this TryOption<T> tryOption) =>
        tryOption.Match(Ok<T>, NotFound, Fail);

    public static IResult ToResult<T>(this Option<T> option) =>
        option.Match(Ok<T>, NotFound);

    public static IResult ToResult<T>(this Fin<T> fin) =>
        fin.Match(Ok<T>, FailWithError);

    public static IResult ToResult<T>(this Eff<T> eff) =>
        eff.Match(Ok<T>, FailWithError).Run()
        .IfFail(FailWithError);

    public static IResult ToResult(this Try<Unit> @try) =>
        @try.Match(NoContent, Fail);

    public static IResult ToResult(this TryOption<Unit> tryOption) =>
        tryOption.Match(NoContent, NotFound, Fail);

    public static IResult ToResult(this Option<Unit> option) =>
        option.Match(NoContent, NotFound);

    public static IResult ToResult(this Fin<Unit> fin) =>
        fin.Match(NoContent, FailWithError);

    public static IResult ToResult(this Eff<Unit> eff) =>
        eff.Match(NoContent, FailWithError).Run()
        .IfFail(FailWithError);
}