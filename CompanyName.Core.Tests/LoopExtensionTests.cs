using Is;

namespace CompanyName.Core.Tests;

[TestClass]
public class LoopExtensionTests
{
	[TestMethod]
	public void From_To()
	{
		4.To(4).Is();
		4.To(9).Is(4, 5, 6, 7, 8);
		9.To(4).Is(9, 8, 7, 6, 5);
	}

	[TestMethod]
	public void From_ToIncluding()
	{
		4.ToIncluding(4).First().Is(4);
		4.ToIncluding(9).Is(4, 5, 6, 7, 8, 9);
		9.ToIncluding(4).Is(9, 8, 7, 6, 5, 4);
	}

	[TestMethod]
	public void From_To_WithStep()
	{
		4.To(9).Step(3).Is(4, 7);
		9.To(4).Step(3).Is(9, 6);
	}

	[TestMethod]
	public void For()
	{
		4.For().Is(0, 1, 2, 3);
		(-4).For().Is(0, -1, -2, -3);
	}
}