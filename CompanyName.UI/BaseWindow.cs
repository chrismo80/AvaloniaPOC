using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CompanyName.UI;

public abstract class BaseWindow : Window
{
	protected BaseWindow()
	{
		// hook into pointer release events from window to check if virtual input controls should be invoked
		if (this.GetSetting<bool>("TouchSupport"))
			AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
	}

	private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (e.Source is Visual visual && visual.FindAncestorOfType<TextBox>() is TextBox textBox)
			textBox.ShowVirtualInputControl();
	}
}