using Is;

namespace CompanyName.Core.Tests;

[TestClass]
public class IsExtensionTestMethods
{
	[TestMethod]
	[DataRow(null, null)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(1, 1)]
	[DataRow(2.2, 2.2)]
	[DataRow(3f, 3f)]
	[DataRow("4", "4")]
	public void Is_Actual_Equals_Expected(object? actual, object? expected) =>
		actual.Is(expected);

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
	public void Actual_Not_Equals_Expected(object? actual, object? expected)
	{
		Action act = () => actual.Is(expected);
		act.IsThrowing<IsNotException>();
	}

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
	public void List_Not_Equal_List()
	{
		Action act = () => new List<int?> { 1, 2, null, 4 }.Is(new List<int?> { 1, 2, 3, 4 });
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	public void Array_Not_Equal_Params()
	{
		Action act = () => new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	public void IEnumerable_Not_Equal_Params_TooShort()
	{
		Action act = () => new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	public void IEnumerable_Not_Equal_Params_TooLong()
	{
		Action act = () => new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	public void IEnumerable_Equal_Params()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);
	}

	[TestMethod]
	public void IsThrowing_Action()
	{
		static int DivideByZero(int value)
		{
			return value / 0;
		}

		Action action = () => _ = DivideByZero(1);
		action.IsThrowing<DivideByZeroException>();
	}

	[TestMethod]
	public void JaggedArrays_Equals_Expected() =>
		new object[] { new[] { 1, 2 }, 3 }.Is(new object[] { new[] { 1, 2 }, 3 });

	[TestMethod]
	public void DifferentDepth_EqualsThough_Expected() =>
		new List<object> { 1, 2 }.Is(new List<object> { 1, new List<object> { 2 } });

	[TestMethod]
	public void DifferentDepth_EqualsExactly_Fails()
	{
		Action act = () => new List<object> { 1, 2 }.IsExactly(new List<object> { 1, new List<object> { 2 } });
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	public void Value_NotEquals_List()
	{
		Action act = () => 5.Is(new List<int> { 1, 2 });
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	public void Value_Is_Type() =>
		new List<int>().Is<IReadOnlyList<int>>();

	[TestMethod]
	public void Value_IsNot_Type()
	{
		Action act = () => new List<int>().Is<IReadOnlyList<double>>();
		act.IsThrowing<IsNotException>();
	}

	[TestMethod]
	[DataRow(3, 4)]
	[DataRow(5, 9)]
	[DataRow(4, -2)]
	[DataRow(-1, 1)]
	[DataRow(0, 5)]
	public void IsGreaterThan_Int_IsSmallerThan(int actual, int expected)
	{
		actual.IsSmallerThan(expected);
		expected.IsGreaterThan(actual);

		Action act = () => actual.IsGreaterThan(expected);
		act.IsThrowing<IsNotException>().Message.Contains("is not greater than").IsTrue();
	}

	[TestMethod]
	[DataRow(3.0, 4.0)]
	[DataRow(5.7, 9.5)]
	[DataRow(-4.0, -2.3)]
	[DataRow(-1.1, 1.2)]
	[DataRow(0.0, 5.3)]
	public void IsGreaterThan_Double_IsSmallerThan(double actual, double expected)
	{
		actual.IsSmallerThan(expected);
		expected.IsGreaterThan(actual);

		Action act = () => actual.IsGreaterThan(expected);
		act.IsThrowing<IsNotException>().Message.Contains("is not greater than").IsTrue();
	}

	[TestMethod]
	public void DateTime()
	{
		var from = new DateTime(2025, 05, 24, 11, 11, 10);
		var to = new DateTime(2025, 05, 24, 11, 11, 11);

		from.IsSmallerThan(to);
		to.IsGreaterThan(from);
	}

	[TestMethod]
	public void Booleans()
	{
		true.IsTrue();
		false.IsFalse();
	}

	[TestMethod]
	public void IsEmpty()
	{
		var list = new List<int>();
		list.IsEmpty();
	}

	[TestMethod]
	public void IsNull()
	{
		List<int>? list = null;
		list.IsNull();
	}

	[TestMethod]
	[DataRow(1.000001, 1.0)]
	[DataRow(2.999999f, 3f)]
	[DataRow(1000000.1, 1000000.0)]
	[DataRow(1_000_000.0, 1_000_001.0)]
	[DataRow(783.0123, 783.0124)]
	[DataRow(1.0 / 3.0, 0.333333)]
	public void IsCloseTo_Actual_Expected(object actual, object expected) =>
		actual.Is(expected);

	private static void VerifyEquality(IEnumerable<int> values)
	{
		values.Is(new int[] { 1, 2, 3, 4 });
		values.Is(new List<int> { 1, 2, 3, 4 });
		values.Is(1, 2, 3, 4);
		values.Where(i => i % 2 == 0).Is(2, 4);
	}
}