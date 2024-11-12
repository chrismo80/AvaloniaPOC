using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System.Reflection;

namespace CompanyName.UI;

public class ViewLocator : IDataTemplate
{
	public bool Match(object? data) => data is BaseViewModel;

	public Control? Build(object? data)
	{
		if (data is null)
			return null;

		var viewName = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

		if ((Type.GetType(viewName) ?? Assembly.GetEntryAssembly()!.GetType(viewName)) is Type type)
		{
			var control = (Control)Activator.CreateInstance(type)!;
			control.DataContext = data;
			return control;
		}

		return new TextBlock { Text = "View not Found: " + viewName };
	}
}