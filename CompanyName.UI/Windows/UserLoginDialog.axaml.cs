using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using CompanyName.Core.Auth;

namespace CompanyName.UI.Windows;

public partial class UserLoginDialog : BaseWindow
{
    readonly IAuthManager? _authManager;

    public UserLoginDialog() => AvaloniaXamlLoader.Load(this);

    public UserLoginDialog(IAuthManager authManager)
        : this()
    {
        _authManager = authManager;

        KeyDown += OnKeyDown;
    }

    public void Show(Window parent, string title)
    {
        Title = title;

        this.FindControl<Label>("Message")!.Content = "";
        
        ShowDialog(parent);

        this.FindControl<TextBox>("UserName")!.Focus();
    }

    private void Validate(object? sender, RoutedEventArgs args)
    {
        var user = this.FindControl<TextBox>("UserName")!.Text!;
        var password = this.FindControl<TextBox>("Password")!.Text!;

        if (_authManager?.Login(user, password) ?? true)
            Close();
        else
            this.FindControl<Label>("Message")!.Content = Assets.Resources.LoginDenied;
    }

    private void Close(object? sender, RoutedEventArgs args)
    {
        Close();
    }

    private void OnKeyDown(object? sender, KeyEventArgs args)
    {
        switch (args.Key)
        {
            case Key.Escape:

                Close(sender, args);

                break;

            case Key.Enter:

                if (this.FindControl<TextBox>("Password")!.IsFocused)
                    Validate(sender, args);

                if (this.FindControl<TextBox>("UserName")!.IsFocused)
                    this.FindControl<TextBox>("Password")!.Focus();

                break;
        }
    }
}