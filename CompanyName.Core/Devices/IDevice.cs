using CompanyName.Core.Messages;

namespace CompanyName.Core.Devices;

public interface IDevice
{
	public int Id { get; }

	public string Name { get; }

	void Execute(string methodName, params object[] parameter)
	{
		this.CreateMessage(Name + " did something", MessageType.Information);
	}

	object GetParameterValue(string parameterName);

	void SetParameterValue(string parameterName, object value);

	bool IsOffline() => false;

	void Reset()
	{
	}
}