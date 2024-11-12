using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using System.Globalization;

namespace CompanyName.UI.Windows;

public partial class Keyboard : Window
{
    readonly Dictionary<string, List<List<string>>> _layouts = new()
    {
        ["en"] = [
            ["qwertyuiop", "asdfghjkl", "zxcvbnm"],
            ["QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM"],
            ["1234567890", "-/:;()€&@\"", ".,?!'"],
            ["[]{}#%^*+=", "_\\|~<>$£¥·", ".,?!'"],
            ],

        ["de"] = [
            ["qwertzuiopü", "asdfghjklöä", "yxcvbnm"],
            ["QWERTZUIOPÜ", "ASDFGHJKLÖÄ", "YXCVBNM"],
            ["1234567890", "-/:;()€&@\"", ".,?!'"],
            ["[]{}#%^*+=", "_\\|~<>$£¥·", ".,?!'"],
            ],
    };

    readonly string _initialText = "";
    readonly int _buttonWidth = 80;

    bool _shift;
    bool _symbols;

    string _culture = "en";

    public List<PixelPoint> AnimationPath { get; private set; } = [];

    public TextBox? Result { get; }

    public Button? Culture { get; }

    public Button? Symbols { get; }

    public Button? Shift { get; }

    public Button? Decimal { get; }

    public Keyboard()
    {
        AvaloniaXamlLoader.Load(this);

        FillRows();
    }

    public Keyboard(string initialText, bool slim = false)
        : this()
    {
        _buttonWidth = slim ? 57 : 80;

        Width = slim ? 795 : 1045;

        KeyDown += OnKeyDown;

        Result = this.FindControl<TextBox>("ResultTextBox")!;
        Culture = this.FindControl<Button>("CultureButton")!;
        Symbols = this.FindControl<Button>("SymbolsButton")!;
        Shift = this.FindControl<Button>("ShiftButton")!;
        Decimal = this.FindControl<Button>("DecimalButton")!;

        _initialText = initialText;

        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        if (_layouts.ContainsKey(culture))
            _culture = culture;

        Culture!.Content = _culture;
        Decimal!.Content = new CultureInfo(_culture).NumberFormat.NumberDecimalSeparator;

        FillRows();
    }

    public static Task<string> Show(Window? parent, string title, string text, char passwordChar)
    {
        var dialog = new Keyboard(text, true);

        dialog.Result!.Text = text;
        dialog.Result.Watermark = title;
        dialog.Result.PasswordChar = passwordChar;

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

    private void Space(object? sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() => Result!.Text += " ");
    }

    private void BackSpace(object? sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (Result!.Text!.Length > 0)
                Result!.Text = Result!.Text.Remove(Result!.Text.Length - 1);
        });
    }

    private void NextCulture(object? sender, RoutedEventArgs args)
    {
        _symbols = _shift = false;

        var languages = _layouts.Keys.ToArray();

        var index = Array.IndexOf(languages, _culture);

        index++;
        index %= languages.Length;

        _culture = languages[index];

        Dispatcher.UIThread.Post(() => Culture!.Content = _culture);
        Dispatcher.UIThread.Post(() => Decimal!.Content = new CultureInfo(_culture).NumberFormat.NumberDecimalSeparator);
        Dispatcher.UIThread.Post(() => Symbols!.Content = "123");

        FillRows();
    }

    private void ToggleShift(object? sender, RoutedEventArgs args)
    {
        _shift = !_shift;

        FillRows();
    }

    private void ToggleSymbols(object? sender, RoutedEventArgs args)
    {
        _symbols = !_symbols;
        _shift = false;

        Dispatcher.UIThread.Post(() => Symbols!.Content = _symbols ? "ABC" : "123");

        FillRows();
    }

    private void AddDecimal(object? sender, RoutedEventArgs args)
    {
        AddCharacter(sender, args);
    }

    private void Abort(object? sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(() => Result!.Text = _initialText);

        Done(sender, args);
    }

    private void Done(object? sender, RoutedEventArgs args)
    {
        int i = 0;

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
        int index = (_symbols ? 2 : 0) + (_shift ? 1 : 0);

        var content = _layouts[_culture][index];

        for (int i = 1; i <= content.Count; i++)
            FillRow("Row" + i, content[i - 1]);
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
            Width = _buttonWidth,
            Height = 80,
            FontSize = 24,
            CornerRadius = new CornerRadius(5),
        };

        button.Click += AddCharacter;

        return button;
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
            case Key.Escape: Abort(sender, args); break;
            case Key.Enter: Done(sender, args); break;
        }
    }
}