namespace Validation.Tests;

[TestClass]
public class ValidatableTypeTest {

    record SampleObject(int Id, string Value);

    class StringType : ValidatableType<StringType, string> {
        public StringType(string value) : base(value) {}
    }

    class SampleObjectType : ValidatableType<SampleObjectType, SampleObject> {
        public SampleObjectType(SampleObject value) : base(value) {}
    }

    class DateTimeType : ValidatableType<DateTimeType, DateTime> {
        public DateTimeType(DateTime value) : base(value) {}
    }

    class IntType : ValidatableType<IntType, int> {
        public IntType(int value) : base(value) {}
    }

    static IEnumerable<object[]> Data => new object[][] {
        new object[] { "Hello World!", typeof(StringType) },
        new object[] { 10, typeof(IntType) },
        new object[] { new SampleObject(10, "foo"), typeof(SampleObjectType) },
        new object[] { DateTime.Now, typeof(DateTimeType) }
    };

    [TestMethod]
    [DynamicData(nameof(Data))]
    public void TryParseTest(object data, Type type) {
        this.GetType().GetMethod(nameof(GenericTryParseTest))
            ?.MakeGenericMethod(type)
            ?.Invoke(null, new object[] { data });

        var nr = StringType.TryParse(null!, out var nvt);
        nr.Should().BeFalse();
        nvt!.Should().BeNull();
    }

    static void GenericTryParseTest<T>(T data, Type type) {
        var method = type.GetMethod("TryParse");
        var args = new object[] { data!, null! };
        var nr = (bool) (method?.Invoke(null, args) ?? false);
        var vt = (T) args[1];
        nr.Should().BeTrue();
        vt!.Should().Be(data);
    }

}