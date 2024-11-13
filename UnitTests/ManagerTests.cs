using CompanyName.Core;

namespace UnitTests;

public abstract class ManagerTests<T>
	where T : Manager, new()
{
	protected T Sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		Sut = new T();

		InitialAsserts();
	}

	[TestCleanup]
	public void Cleanup() => Sut.Dispose();

	protected abstract void InitialAsserts();
}