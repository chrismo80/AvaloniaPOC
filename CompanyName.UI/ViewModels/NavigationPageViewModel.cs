using CommunityToolkit.Mvvm.ComponentModel;

namespace CompanyName.UI.ViewModels;

public abstract partial class NavigationPageViewModel : PageViewModel
{
	readonly PageViewModel[] _pages = [];

	int _index;

	[ObservableProperty]
	PageViewModel? _currentPage;

	[ObservableProperty]
	bool _prevEnabled;

	[ObservableProperty]
	bool _nextEnabled;

	protected NavigationPageViewModel(params PageViewModel[] pages)
	{
		_pages = pages;

		if (pages.Any())
			SetPage();
	}

	public void SelectPage(string page) =>
		SetPage(Array.FindIndex(_pages, p => p.Name == page));

	public void PrevPage() => SetPage(_index - 1);

	public void NextPage() => SetPage(_index + 1);

	private void SetPage(int index = 0)
	{
		if (index < 0)
			return;

		_index = Math.Min(Math.Max(index, 0), _pages.Length - 1);

		CurrentPage = _pages[_index];

		PrevEnabled = _index > 0;
		NextEnabled = _index < _pages.Length - 1;
	}
}