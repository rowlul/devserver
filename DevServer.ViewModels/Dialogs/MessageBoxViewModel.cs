using CommunityToolkit.Mvvm.Input;

namespace DevServer.ViewModels;

public enum MessageBoxIcon
{
    Warning = 179,
    Error = 182,
    Question = 3610
}

public enum MessageBoxButtons
{
    Ok,
    OkCancel
}

public partial class MessageBoxViewModel : ObservableDialog
{
    public string Title { get; }
    public string Text { get; }
    public MessageBoxIcon? Icon { get; }
    public MessageBoxButtons? Buttons { get; }
    public bool IsCancelVisible => Buttons == MessageBoxButtons.OkCancel;

    public MessageBoxViewModel(string title, string text,
                               MessageBoxIcon? icon = null,
                               MessageBoxButtons? buttons = MessageBoxButtons.Ok)
    {
        Title = title;
        Text = text;
        Icon = icon;
        Buttons = buttons;
    }

    [RelayCommand]
    public void Ok()
    {
        DialogResult = true;
        base.Close();
    }

    protected override void Close()
    {
        DialogResult = false;
        base.Close();
    }
}
