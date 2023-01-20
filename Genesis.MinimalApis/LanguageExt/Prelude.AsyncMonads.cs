namespace Genesis;

using LanguageExt.Common;

public static partial class Prelude {
    
    public static Task<IResult> ToResultAsync<T>(this TryAsync<T> tryAsync) =>
        tryAsync.Match(Ok<T>, Fail);

    public static Task<IResult> ToResultAsync<T>(this TryOptionAsync<T> tryOptionAsync) =>
        tryOptionAsync.Match(Ok<T>, NotFound, Fail);

    public static Task<IResult> ToResultAsync<T>(this OptionAsync<T> optionAsync) =>
        optionAsync.Match(Ok<T>, NotFound);

    public static ValueTask<IResult> ToResultAsync<T>(this Aff<T> aff) =>
        aff.Match(Ok<T>, Error).Run()
        .Map(r => r.IfFail(Error));

    public static Task<IResult> ToResultAsync(this TryAsync<Unit> tryAsync) =>
        tryAsync.Match(Ok, Fail);

    public static Task<IResult> ToResultAsync(this TryOptionAsync<Unit> tryOptionAsync) =>
        tryOptionAsync.Match(Ok, NotFound, Fail);

    public static Task<IResult> ToResultAsync(this OptionAsync<Unit> optionAsync) =>
        optionAsync.Match(Ok, NotFound);

    public static ValueTask<IResult> ToResultAsync(this Aff<Unit> aff) =>
        aff.Match(Ok<Unit>, Error).Run()
        .Map(r => r.IfFail(Error));
}