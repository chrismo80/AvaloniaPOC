using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Media;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CompanyName.Core.Devices;

namespace CompanyName.UI.DataTemplates;

public partial class Chart : ObservableObject
{
	readonly IDevice? _device;
	readonly List<DateTimePoint> _values = [];

	readonly Random _random = new();
	readonly DateTimeAxis _customAxis;

	readonly int _offset;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BackColor))]
	double _value;

	public ObservableCollection<ISeries> Series { get; set; } = [];

	public ObservableCollection<RectangularSection> Sections { get; set; } = [];

	public string Name { get; } = "";

	public IBrush BackColor => GetStateColor();

	public bool OutOfRange => Value < Min || Value > Max;

	public int RefreshRate { get; set; }

	public int Min { get; set; }

	public int Max { get; set; }

	public int Scale { get; set; }

	public int Duration { get; set; }

	public LiveChartsCore.SkiaSharpView.Axis[] XAxes { get; set; }

	public LiveChartsCore.SkiaSharpView.Axis[] YAxes { get; set; }

	public object Sync { get; } = new object();

	public Chart() : this(null, "Chart")
	{
	}

	public Chart(IDevice? device, string name, string deviceParameter = "", int refreshRate = 100, int min = -10, int max = 10, int scale = 3,
		int duration = 60)
	{
		Name = name;

		Min = min;
		Max = max;
		Scale = scale;
		Duration = duration;
		RefreshRate = refreshRate;

		_device = device;

		_offset = (Max - Min) * Scale;

		_values.Add(new DateTimePoint(DateTime.Now, Min + (Max - Min) / 2));

		_device!.SetParameterValue("SomeName", _values.Last().Value!);

		Series = new ObservableCollection<ISeries>
		{
			new LineSeries<DateTimePoint>
			{
				Values = _values,
				Fill = null,
				GeometryFill = null,
				GeometryStroke = null,
				LineSmoothness = 0.5,

				Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 },
			},
		};

		Sections = new ObservableCollection<RectangularSection>
		{
			new RectangularSection
			{
				Fill = new SolidColorPaint(SKColors.PaleTurquoise.WithAlpha(100)),

				//Stroke = new SolidColorPaint(SKColors.PaleTurquoise) { StrokeThickness = 2},
				Yi = Min,
				Yj = Max,
			},
			new RectangularSection
			{
				Fill = new SolidColorPaint(SKColors.LightPink.WithAlpha(50)),
				Yi = Max,
				Yj = Max + _offset,
			},
			new RectangularSection
			{
				Fill = new SolidColorPaint(SKColors.LightPink.WithAlpha(50)),
				Yi = Min - _offset,
				Yj = Min,
			},
		};

		_customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
		{
			CustomSeparators = GetSeparators(),
			AnimationsSpeed = TimeSpan.FromMilliseconds(0),

			SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
			LabelsPaint = new SolidColorPaint(SKColors.Gray),
			TextSize = 20,
		};

		XAxes = [_customAxis];

		YAxes =
		[
			new LiveChartsCore.SkiaSharpView.Axis
			{
				MinLimit = Min - _offset,
				MaxLimit = Max + _offset,

				SeparatorsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(100)),
				LabelsPaint = new SolidColorPaint(SKColors.Gray),
				TextSize = 20,
			}
		];

		_ = ReadData();
	}

	private static string Formatter(DateTime date)
	{
		var secsAgo = (DateTime.Now - date).TotalSeconds;

		return secsAgo < 1 ? DateTime.Now.ToString("HH:mm:ss") : $"{secsAgo:N0}s";
	}

	private double NextValue()
	{
		return (double)_device!.GetParameterValue("SomeParameterNameDoesntMatter");
	}

	private async Task ReadData()
	{
		var sw = Stopwatch.StartNew();

		while (sw.Elapsed.TotalSeconds < Duration * 10)
		{
			await Task.Delay(RefreshRate);

			Value = NextValue();

			lock (Sync)
			{
				_values.Add(new DateTimePoint(DateTime.Now, Value));

				if ((DateTime.Now - _values.First().DateTime).TotalSeconds > Duration)
					_values.RemoveAt(0);

				_customAxis.CustomSeparators = GetSeparators();
			}
		}
	}

	private double[] GetSeparators()
	{
		var now = DateTime.Now;

		var separators = new List<double>();

		for (int i = -1 * Duration; i <= 0; i += 5)
			separators.Add(now.AddSeconds(i).Ticks);

		return separators.ToArray();
	}

	private IBrush GetStateColor() => OutOfRange switch
	{
		true => Brushes.Maroon,
		false => Brushes.Transparent,
	};
}