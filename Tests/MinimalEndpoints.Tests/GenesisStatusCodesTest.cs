namespace MinimalEndpoints.Tests;

using Microsoft.AspNetCore.Http;
using System.Reflection;
using LanguageExt;

[TestClass]
public class GenesisStatusCodesTest
{

    [TestMethod]
    public void ContainsAllCodes() =>
        StatusCodes.Should().HaveCount(HttpStatusCodes.Default.Count);
    
    [TestMethod]
    [DynamicData(nameof(Fields))]
    public void StatusCodeTest(int statusCode) {
        var (code, type) = HttpStatusCodes.StatusCodes
            .First(n => n.Status == statusCode);
        type.Should().Be($"https://httpwg.org/specs/rfc9110.html#status.{code}");
        StatusCodes.Should().Contain(code);
    }

    static IEnumerable<object[]> Fields =>
        typeof(StatusCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => (int) fi.GetValue(null)!)
            .Distinct().Select(n => new object[] { n }).Memo();

    readonly static Seq<int> StatusCodes =
        Fields.Select(n => n.First()).Cast<int>().ToSeq();
}