using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace CompanyName.UI.Windows;

public partial class NumPad : Window
{
    readonly List<string> _numbers = ["789", "456", "123", "-0" + Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator];

    readonly string _initialText = "";

    public List<PixelPoint> AnimationPath { get; private set; } = [];

    public TextBox? Result { get; }

    public NumPad()
    {
        AvaloniaXamlLoader.Load(this);

        FillRows();
    }

    public NumPad(string initialText)
        : this()
    {
        KeyDown += OnKeyDown;

        Result = this.FindControl<TextBox>("ResultTextBox")!;

        _initialText = initialText;
    }

    public static Task<string> Show(Window? parent, string title, string text = "")
    {
        var dialog = new NumPad(text) { Title = title };

        dialog.Result!.Text = text;

        var tcs = new TaskCompletionSource<string>();

        dialog.Closed += (_, _) => tcs.TrySetResult(dialog.Result.Text ?? "");

        if (parent == null)
            dialog.Show();
        else
            dialog.ShowDialog(parent);

        dialog.GetAnimationPath(15);
        dialog.Position = dialog.AnimationPath[0];
        dialog.Result.Focus();

        dialog.Animate();

        return tcs.Task;
    }

    private static double EaseOutQuad(double t) => t * (2 - t);

    private static IEnumerable<double> GetPath(double start, double end, int steps)
    {
        for (int i = 0; i <= steps; i++)
            yield return EaseOutQuad((double)i / steps) * (end - start) + start;
    }

    private void GetAnimationPath(int steps)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Avalonia.Platform.Screen screen = desktop.MainWindow!.Screens.Primary!;

            PixelSize screenSize = screen.WorkingArea.Size;
            PixelSize windowSize = PixelSize.FromSize(ClientSize, screen.Scaling);

            foreach (var step in GetPath(screenSize.Height, screenSize.Height - windowSize.Height, steps))
                AnimationPath.Add(new PixelPoint(screenSize.Width / 2 - windowSize.Width / 2, (int)step));
        }
    }

    private void Animate()
    {
        int i = 0;

        var timer = new DispatcherTimer(DispatcherPriority.Background);

        timer.Tick += (_, _) =>
        {
            Position = AnimationPath[i++];

            if (i >= AnimationPath.Count)
                timer.Stop();
        };

        timer.Start();
    }

    private void Abort(object? sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() => Result!.Text = _initialText);
        Done(sender, args);
    }

    private void Done(object? sender, RoutedEventArgs args)
    {
        int i = 1;

        AnimationPath.Reverse();

        var timer = new DispatcherTimer(DispatcherPriority.Background);

        timer.Tick += (_, _) =>
        {
            Position = AnimationPath[i++];

            if (i >= AnimationPath.Count)
            {
                timer.Stop();
                Close();
            }
        };

        timer.Start();
    }

    private void FillRows()
    {
        for (int i = 1; i <= _numbers.Count; i++)
            FillRow("Row" + i, _numbers[i - 1]);
    }

    private void FillRow(string stackPanelName, string characters)
    {
        var panel = this.FindControl<StackPanel>(stackPanelName)!;

        panel.Children.Clear();

        foreach (var character in characters)
            panel.Children.Add(CreateButton(character));
    }

    private Button CreateButton(char text)
    {
        var button = new Button
        {
            Content = text,
            CornerRadius = new CornerRadius(5),
            Height = 80,
            Width = 80,
            FontSize = 24
        };

        button.Click += AddCharacter;

        return button;
    }

    private void BackSpace(object? sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (Result!.Text!.Length > 0)
                Result!.Text = Result!.Text.Remove(Result!.Text.Length - 1);
        });
    }

    private void AddCharacter(object? sender, RoutedEventArgs args)
    {
        if (sender is Button button)
            Dispatcher.UIThread.Post(() => Result!.Text += button.Content?.ToString() ?? "");
    }

    private void OnKeyDown(object? sender, KeyEventArgs args)
    {
        switch (args.Key)
        {
            case Key.Enter: Done(sender, args); break;
            case Key.Escape: Abort(sender, args); break;
        }
    }
}