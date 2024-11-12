using Avalonia.Media;
using Avalonia.Platform;

namespace CompanyName.UI.DataTemplates;

public class ParameterMatchPair
{
	readonly IBrush[] _colors =
	[
		new SolidColorBrush(Colors.DarkOliveGreen),
		new SolidColorBrush(Colors.Maroon),
	];

	public Avalonia.Media.Imaging.Bitmap? Image { get; }

	public string Name { get; }

	public double DisplayMin => Min - (Max - Min) / 2;

	public double DisplayMax => Max + (Max - Min) / 2;

	public double Min { get; }

	public double Max { get; }

	public double Value { get; }

	public string Unit { get; }

	public bool Success { get; }

	public IBrush Color => Success ? _colors[0] : _colors[1];

	public ParameterMatchPair(Core.Data.ParameterMatch? pair)
	{
		Name = (pair?.Name ?? "Temp") + ":";
		Min = pair?.Min ?? 1;
		Max = pair?.Max ?? 2;
		Value = pair?.Value ?? 3;
		Unit = pair?.Unit ?? "°C";
		Success = pair?.Success ?? true;

		var imageName = Success ? "success" : "fail";

		Image = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri($"avares://CompanyName.UI/Assets/{imageName}.png")));
	}

	public ParameterMatchPair() : this(null)
	{
	}
}