using CompanyName.Core;

namespace UnitTests;

public abstract class ManagerTests<T>
	where T : BaseService, new()
{
	protected T Sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		Sut = new T();

		Pre();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Sut.Dispose();
	}

	protected abstract void Pre();
}