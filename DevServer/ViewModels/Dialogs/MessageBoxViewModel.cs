using CommunityToolkit.Mvvm.Input;

using Material.Icons;

namespace DevServer.ViewModels.Dialogs;

public enum MessageBoxButtons
{
    Ok,
    OkCancel
}

public partial class MessageBoxViewModel : DialogViewModelBase
{
    public string Title { get; }
    public string Text { get; }
    public MaterialIconKind? Icon { get; }
    public MessageBoxButtons Buttons { get; }
    public bool IsCancelVisible => Buttons == MessageBoxButtons.OkCancel;

    public MessageBoxViewModel(string title, string text,
                               MaterialIconKind? icon = null,
                               MessageBoxButtons buttons = MessageBoxButtons.Ok)
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
