using System.ComponentModel;

namespace CompanyName.Core.Models;

public abstract class BaseModel : INotifyPropertyChanged, IExecutable
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public abstract Task Execute();

	protected void OnPropertyChanged(string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	protected void Guard(bool condition, string message) => this.IsTrue(condition, message);
}