namespace ValidationTests;

using Genesis;
using Genesis.Validation;
using global::Microsoft.AspNetCore.Http;
using System.Reflection;
using FluentAssertions;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1() {
        var fields = typeof(StatusCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => (int) fi.GetValue(null))
            .Distinct();

        var codes = GenesisStatusCodes.Default;

        fields.Count().Should().Be(codes.Count);

        foreach(var (code, type) in codes)
            type.Should().Be($"https://httpwg.org/specs/rfc9110.html#status.{code}");
    }
}