using CompanyName.Core.Is;

namespace CompanyName.Core.Tests;

[TestClass]
public class IsExtensionTests
{
    [TestMethod]
    [DataRow(null, null)]
    [DataRow(false, false)]
    [DataRow(true, true)]
    [DataRow(1, 1)]
    [DataRow(2.2, 2.2)]
    [DataRow(3f, 3f)]
    [DataRow("4", "4")]
    public void Is_Actual_Equals_Expected(object actual, object expected) =>
        actual.Is(expected);

    [TestMethod]
    [DataRow(null, null)]
    [DataRow(false, false)]
    [DataRow(true, true)]
    [DataRow(1, 1)]
    [DataRow(2.2, 2.2)]
    [DataRow(3f, 3f)]
    [DataRow("4", "4")]
    public void IsNot_Actual_Equals_Expected(object actual, object expected) =>
        Assert.ThrowsException<Exception>(() => actual.IsNot(expected));

    [TestMethod]
    [DataRow(null, true)]
    [DataRow(null, false)]
    [DataRow(true, false)]
    [DataRow(5, null)]
    [DataRow(6, 6d)]
    [DataRow(7, true)]
    [DataRow(false, 7d)]
    [DataRow(8d, 8f)]
    [DataRow(99, "99")]
    [DataRow("ABC", false)]
    [DataRow("ABC", null)]
    [DataRow("ABC", "ABD")]
    public void Actual_Not_Equals_Expected(object actual, object expected) =>
        Assert.ThrowsException<Exception>(() => actual.Is(expected));

    [TestMethod]
    public void ListValues_Equal_Expected() =>
        VerifyEquality(new List<int> { 1, 2, 3, 4 });

    [TestMethod]
    public void ArrayValues_Equal_Expected() =>
        VerifyEquality(new int[] { 1, 2, 3, 4 });

    [TestMethod]
    public void ValuesWithNull_Equal_Expected() =>
        new int?[] { 1, 2, null, 4 }.Is(1, 2, null, 4);

    [TestMethod]
    public void List_Not_Equal_List() =>
        Assert.ThrowsException<Exception>(() => new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 }));

    [TestMethod]
    public void Array_Not_Equal_Params() =>
        Assert.ThrowsException<Exception>(() => new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 }));

    [TestMethod]
    public void IEnumerable_Not_Equal_Params_TooShort() =>
        Assert.ThrowsException<Exception>(() => new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4));

    [TestMethod]
    public void IEnumerable_Not_Equal_Params_TooLong() =>
        Assert.ThrowsException<Exception>(() => new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4));

    [TestMethod]
    public void JaggedArrays_Equals_Expected() =>
        new object[] { new[] { 1, 2 }, 3 }.Is(new object[] { new[] { 1, 2 }, 3 });

    [TestMethod]
    public void DifferentDepth_EqualsThough_Expected() =>
        new List<object> { 1, 2 }.Is(new List<object> { 1, new List<object> { 2 } });

    [TestMethod]
    public void Value_NotEquals_List() =>
        Assert.ThrowsException<Exception>(() => 5.Is(new List<int> { 1, 2 }));

    [TestMethod]
    public void Value_Is_Type() =>
        new List<int>().Is<IReadOnlyList<int>>();

    [TestMethod]
    public void Value_IsNot_Type() =>
        Assert.ThrowsException<Exception>(() => new List<int>().Is<IReadOnlyList<double>>());

    private static void VerifyEquality(IEnumerable<int> values)
    {
        values.Is(new int[] { 1, 2, 3, 4 });
        values.Is(new List<int> { 1, 2, 3, 4 });
        values.Is(1, 2, 3, 4);
        values.Where(i => i % 2 == 0).Is(2, 4);
    }
}