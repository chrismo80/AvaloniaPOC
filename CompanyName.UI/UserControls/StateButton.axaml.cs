using Avalonia.Controls;
using Avalonia.Media;

namespace CompanyName.UI.UserControls;

public partial class StateButton : Button
{
	private int _state;

	public StateButton()
	{
		Initialized += (_, _) => UpdateState();
	}

	protected override void OnClick()
	{
		base.OnClick();

		_state++;
		_state %= 3;

		UpdateState();
	}

	private void UpdateState()
	{
		switch (_state)
		{
			case 0:
				Content = "State 0";
				Background = Brushes.DarkKhaki;
				break;
			case 1:
				Content = "State 1";
				Background = Brushes.DarkOrange;
				break;
			case 2:
				Content = "State 2";
				Background = Brushes.DarkRed;
				break;
		}
	}
}