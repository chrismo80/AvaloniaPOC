using CompanyName.Core;

namespace UnitTests;

public abstract class ManagerTests<T> where T : Manager
{
	protected T _sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		_sut = Activator.CreateInstance<T>();

		InitialAsserts();
	}

	[TestCleanup]
	public void Cleanup() => _sut.Dispose();

	protected abstract void InitialAsserts();
}