using CompanyName.Core;

namespace UnitTests;

public abstract class ManagerTests<T> where T : Manager
{
	protected T Sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		Sut = Activator.CreateInstance<T>();

		InitialAsserts();
	}

	[TestCleanup]
	public void Cleanup() => Sut.Dispose();

	protected abstract void InitialAsserts();
}