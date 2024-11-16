using CompanyName.Core;

namespace UnitTests;

public abstract class ServiceTests<T>
	where T : BaseService
{
	protected T Sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		Sut = (T)Activator.CreateInstance(typeof(T), true)!;

		Pre();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Post();

		Sut.Dispose();
	}

	protected virtual void Pre()
	{
	}

	protected virtual void Post()
	{
	}
}