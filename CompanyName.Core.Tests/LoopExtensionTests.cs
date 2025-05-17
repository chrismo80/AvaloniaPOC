using CompanyName.Core.Is;

namespace CompanyName.Core.Tests;

[TestClass]
public class LoopExtensionTests
{
	[TestMethod]
	public void From_To_Verify()
	{
		1.To(9).Is(1, 2, 3, 4, 5, 6, 7, 8, 9);
		1.ToExcluding(9).Is(1, 2, 3, 4, 5, 6, 7, 8);
		1.To(9).Step(3).Is(1, 4, 7);
	}
}