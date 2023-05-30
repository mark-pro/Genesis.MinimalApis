
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MinimalEndpoints.Tests;

[TestClass]
public class ExceptionExtensionTests {

    static object[][] EXCEPTIONS =>
        new[] {
            new object[] { new ArgumentException("String cannot be empty", "echo"), StatusCodes.Status400BadRequest },
            new object[] { new ValidationException("Property or field is required" , new RequiredAttribute(), null), StatusCodes.Status400BadRequest },
            new object[] { new NullReferenceException(), StatusCodes.Status500InternalServerError },
            new object[] { new Exception("An unhandled error occurred"), StatusCodes.Status500InternalServerError }
        };

    [TestMethod]
    [DynamicData(nameof(EXCEPTIONS), DynamicDataSourceType.Property)]
    public void ToProblemDetailsTest(Exception e, int statusCode) {
        var details = e.ToProblemDetails();

        var be = (object expected, object? actual) =>
            actual.Should().Be(expected);

        be(statusCode, details.Status);
        be(e.Message, details?.Detail);

        ((e, details) switch {
            (ValidationException, ValidationProblemDetails) => (Action<Func<string, AndConstraint<StringAssertions>>>)
                (x => x("One or more validation errors have occurred.")),
            (ArgumentException, ValidationProblemDetails) => x => x("Invalid argument provided."),
            (Exception, ProblemDetails) => x => x("An unexpected error has occurred."),
#if NET7_0_OR_GREATER
            _ => throw new UnreachableException()
#else
            _ => throw new Exception("Code should not be reached")
#endif
        })((string s) => details.Title.Should().Be(s));
    }

    [TestMethod]
    [DynamicData(nameof(Depths), DynamicDataSourceType.Property)]
    public void DepthTest(ushort depth) {

        static Exception exceptionGenerator(ushort depth) {
            var e = new Exception("Exception message 0");
            for (ushort i = 0; i < depth; i++)
                e = new($"Exception message {i + 1}", e);

            return e;
        }

        var count = (ushort)Depths.Length;

        var pd = exceptionGenerator(count).ToProblemDetails(maxMessageDepth: depth);
        pd.Detail.Should().Be($"Exception message {count - depth}");
    }

    static object[][] Depths =>
        Enumerable.Range(0, 5)
        .Select(i => new object[] { (ushort) i })
        .ToArray();
}

