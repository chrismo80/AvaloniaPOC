using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CompanyName.UI.DataTemplates;

public partial class Axis : ObservableObject
{
	readonly IBrush[] _colors =
	[
		new SolidColorBrush(Colors.DarkOliveGreen, 0.6),
		new SolidColorBrush(Colors.Maroon, 0.6),
	];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BackColor))]
	string _state = "";

	[ObservableProperty]
	double _position;

	public string Name { get; } = "";

	public double TargetPosition { get; set; }

	public IBrush BackColor => GetStateColor();

	public Axis() : this("Axis", "Ready", 1.1)
	{
	}

	public Axis(string name, string state, double position)
	{
		Name = name;
		State = state;
		Position = position;
	}

	public void ChangeState()
	{
		State = State switch
		{
			"Ready" => "Busy",
			"Busy" => "Error",
			"Error" => "Ready",
			_ => "Ready",
		};
	}

	public async void Move()
	{
		if (State == "Error")
		{
			State = "Ready";
			return;
		}

		if (State == "Ready")
			State = "Busy";

		await Task.Delay(1000);

		Position = TargetPosition;

		State = "Error";
	}

	private IBrush GetStateColor() => State switch
	{
		"Ready" => _colors[0],
		"Busy" => Brushes.Transparent,
		"Error" => _colors[1],
		_ => Brushes.Transparent,
	};
}