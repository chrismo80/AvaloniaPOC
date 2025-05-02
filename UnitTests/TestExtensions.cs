using System.Collections;

namespace UnitTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

public static class TestExtensions
{
    /// <summary>
    /// checks if two objects are equal or have equal values in cases of enumerables
    /// </summary>
    /// <returns>throws exception if not, never returns false</returns>
    public static bool Is(this object value, params object[]? expected)
    {
        expected = expected?.Unwrap();

        return (expected?.Length) switch
        {
            null => value.IsEqualTo(expected),
            1 => value.IsEqualTo(expected[0]),
            _ => value.ToEnumerable().Are(expected),
        };
    }

    private static object[]? Unwrap(this object[]? array)
    {   // unwraps inner array of first element (due to params usage)
        if (array?.Length == 1 && array[0] is IEnumerable list && array[0] is not string)
            return list.ToEnumerable().ToArray();

        return array;
    }

    private static IEnumerable<object> ToEnumerable(this object list) =>
        (list as IEnumerable).Cast<object>();

    private static bool Are(this IEnumerable<object> values, IEnumerable<object> expected) =>
        Enumerable.Range(0, Math.Max(values.Count(), expected.Count()))
            .All(i => values.ElementAt(i).Is(expected.ElementAt(i)));

    private static bool IsEqualTo(this object? value, object? expected)
    {
        Assert.AreEqual(expected, value);
        return true;
    }
}

[TestClass]
public class TestExtensionTests
{
    [TestMethod]
    [DataRow(null, null)]
    [DataRow(false, false)]
    [DataRow(true, true)]
    [DataRow(1, 1)]
    [DataRow(2.2, 2.2)]
    [DataRow(3f, 3f)]
    [DataRow("4", "4")]
    public void Value_Equals_Expected(object value, object expected) =>
        value.Is(expected);

    [TestMethod]
    [ExpectedException(typeof(AssertFailedException))]
    [DataRow(null, true)]
    [DataRow(null, false)]
    [DataRow(true, false)]
    [DataRow(5, null)]
    [DataRow(6, 6d)]
    [DataRow(7, true)]
    [DataRow(false, 7d)]
    [DataRow(8d, 8f)]
    [DataRow(9, "9")]
    [DataRow("ABC", false)]
    [DataRow("ABC", null)]
    [DataRow("ABC", "ABD")]
    public void Value_Not_Equals_Expected(object value, object expected)
    {
        value.Is(expected);
        expected.Is(value);
    }

    [TestMethod]
    public void ListValues_Equal_Expected()
    {
        var values = new List<int> { 1, 2, 3, 4 };

        values.Is(new int[] { 1, 2, 3, 4 });
        values.Is(new List<int> { 1, 2, 3, 4 });
        values.Is(1, 2, 3, 4);
        values.Where(i => i % 2 == 0).Is(2, 4);
    }

    [TestMethod]
    public void ArrayValues_Equal_Expected()
    {
        var values = new int[] { 1, 2, 3, 4 };

        values.Is(new int[] { 1, 2, 3, 4 });
        values.Is(new List<int> { 1, 2, 3, 4 });
        values.Is(1, 2, 3, 4);
        values.Where(i => i % 2 == 0).Is(2, 4);
    }

    [TestMethod]
    [ExpectedException(typeof(AssertFailedException))]
    public void List_Not_Equal_List() =>
        new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });

    [TestMethod]
    [ExpectedException(typeof(AssertFailedException))]
    public void Array_Not_Equal_Params() =>
        new int[] { 1, 2, 3, 5 }.Is(1, 2, 3, 4);

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IEnumerable_Not_Equal_Params_TooLong() =>
        new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IEnumerable_Not_Equal_Params_TooShort() =>
        new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);
}