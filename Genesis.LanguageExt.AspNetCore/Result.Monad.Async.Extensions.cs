using LanguageExt.Effects.Traits;

namespace Genesis;

public static partial class Core {
    
    public static Task<IResult> ToResultAsync<T>(this TryAsync<T> tryAsync) =>
        tryAsync.Match(Success, Fail);

    public static Task<IResult> ToResultAsync<T>(this TryOptionAsync<T> tryOptionAsync) =>
        tryOptionAsync.Match(Success, NotFound, Fail);

    public static Task<IResult> ToResultAsync<T>(this OptionAsync<T> optionAsync) =>
        optionAsync.Match(Success, NotFound);

    public static ValueTask<IResult> ToResultAsync<T>(this Aff<T> aff) =>
        aff.Map(Success)
        .Run()
        .Map(r => r.ToResult());

    public static ValueTask<IResult> ToResultAsync<RT, T>(this Aff<RT, T> aff, RT runtime) where RT : struct, HasCancel<RT> =>
        aff.Map(Success)
        .Run(runtime)
        .Map(r => r.ToResult());
}