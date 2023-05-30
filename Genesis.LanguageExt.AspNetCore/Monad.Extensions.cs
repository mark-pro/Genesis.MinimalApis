using LanguageExt.Common;

namespace Genesis;

public static partial class Core {

    /// <summary>
    /// Converts and <seealso cref="Exception" /> to a <seealso cref="ProblemDetails" />
    /// </summary>
    readonly static Func<Exception, ProblemDetails> ToProblemDetails =
        e => e.ToProblemDetails();

    /// <summary>
    /// Converts an <seealso cref="Error" /> to an <seealso cref="Exception" />
    /// </summary>
    readonly static Func<Error, Exception> ToException =
        e => e.ToException();
}