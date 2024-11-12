using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CompanyName.UI.Windows;

public partial class MessageBox : Window
{
    public enum MessageBoxButtons
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        Ok,
        Cancel,
        Yes,
        No
    }

    public MessageBox()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
    {
        var dialog = new MessageBox()
        {
            Title = title
        };

        dialog.FindControl<TextBlock>("Text")!.Text = text;

        var buttonPanel = dialog.FindControl<StackPanel>("Buttons");

        var result = MessageBoxResult.Ok;

        void AddButton(string caption, MessageBoxResult dialogResult, bool def = false)
        {
            var button = new Button { Content = caption };

            button.MinHeight = 50;
            button.MinWidth = 100;
            button.FontSize = 18;

            button.Click += (_, _) =>
            {
                result = dialogResult;
                dialog.Close();
            };

            buttonPanel!.Children.Add(button);

            if (def)
                result = dialogResult;
        }

        if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
            AddButton("Cancel", MessageBoxResult.Cancel, true);

        if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
            AddButton("Ok", MessageBoxResult.Ok, true);

        if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
        {
            AddButton(Assets.Resources.No, MessageBoxResult.No, true);
            AddButton(Assets.Resources.Yes, MessageBoxResult.Yes);
        }

        var tcs = new TaskCompletionSource<MessageBoxResult>();

        dialog.Closed += delegate { tcs.TrySetResult(result); };

        if (parent != null)
            dialog.ShowDialog(parent);
        else
            dialog.Show();

        return tcs.Task;
    }
}