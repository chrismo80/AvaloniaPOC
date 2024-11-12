using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using CompanyName.Core.Data;

namespace ProjectExampleHMI.ViewModels;

public partial class SystemOverviewViewModel : CompanyName.UI.ViewModels.PageViewModel
{
	[ObservableProperty]
	Avalonia.Media.Imaging.Bitmap? _someBitmap;

	public ObservableCollection<CompanyName.UI.DataTemplates.ParameterMatchPair> Measurements { get; private set; } = [];

	public SystemOverviewViewModel()
	{
		Name = "Overview";
		Message = "This is the System Overview ViewModel";

		SomeBitmap = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://CompanyName.UI/Assets/Logo.png")));

		GenerateMatch();
	}

	public void GenerateMatch()
	{
		var random = new Random();

		var pairs = Enumerable.Range(1, random.Next(3, 8))
			.Select(i => new ParameterMatch($"Messung # {i}", random.NextDouble() * 50, random.NextDouble() * 50 + 50, random.NextDouble() * 100,
				"mm"))
			.ToList();

		// change inner list, do not replace whole observable collection, only collection items are observed by the view
		Measurements.Clear();

		foreach (var pair in pairs)
			Measurements.Add(new CompanyName.UI.DataTemplates.ParameterMatchPair(pair));
	}
}