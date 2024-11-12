using System.ComponentModel;
using CompanyName.Core.Messages;

namespace CompanyName.Core.Devices;

public abstract class BaseDevice : INotifyPropertyChanged, IDevice
{
	private int _executions;

	public event PropertyChangedEventHandler? PropertyChanged;

	public int Executions
	{
		get => _executions;
		set
		{
			if (_executions == value)
				return;

			_executions = value;
			OnPropertyChanged(nameof(Executions));
		}
	}

	public bool Online { get; protected set; }

	public string Name { get; protected set; }

	public int Id => 0;

	protected BaseDevice()
	{
		Name = GetType().Name;

		Reset().GetAwaiter().GetResult();
	}

	public Task Reset()
	{
		return Task.Run(() =>
		{
			try
			{
				Init();
				Online = true;
			}
			catch (Exception ex)
			{
				this.CreateMessage(ex);
				Online = false;
			}
		});
	}

	public object GetParameterValue(string parameterName)
	{
		throw new NotImplementedException("GetParameterValue");
	}

	public void SetParameterValue(string parameterName, object value)
	{
		throw new NotImplementedException("SetParameterValue");
	}

	protected void OnPropertyChanged(string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	protected virtual void Init()
	{
		Task.Delay(200).Wait();

		Guard(DateTime.Now.Microsecond <= 300, "Init failed");
	}

	protected void Guard(bool condition, string message) => this.IsTrue(condition, message);
}