using LanguageExt.Common;

namespace Genesis;

public static partial class Core {
    readonly static Func<Exception, ProblemDetails> ToProblemDetails =
        e => e.ToProblemDetails();

    readonly static Func<Error, Exception> ToException =
        e => e.ToException();
}