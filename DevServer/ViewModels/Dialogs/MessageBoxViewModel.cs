using System;

using CommunityToolkit.Mvvm.Input;

using Material.Icons;

namespace DevServer.ViewModels.Dialogs;

public enum MessageBoxIcon
{
    Warning = MaterialIconKind.Warning,
    Error = MaterialIconKind.Error,
    Question = MaterialIconKind.QuestionMarkCircle
}

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
    public MessageBoxButtons? Buttons { get; }
    public bool IsCancelVisible => Buttons == MessageBoxButtons.OkCancel;

    public MessageBoxViewModel(string title, string text,
                               MessageBoxIcon? icon = null,
                               MessageBoxButtons? buttons = MessageBoxButtons.Ok)
    {
        Title = title;
        Text = text;
        Buttons = buttons;

        if (icon.HasValue)
        {
            Icon = MapToMaterialIconKind(icon.Value);
        }
    }

    private static MaterialIconKind MapToMaterialIconKind(MessageBoxIcon icon) => icon switch
    {
        MessageBoxIcon.Warning => MaterialIconKind.Warning,
        MessageBoxIcon.Error => MaterialIconKind.Error,
        MessageBoxIcon.Question => MaterialIconKind.QuestionMarkCircle,
        _ => throw new ArgumentOutOfRangeException(nameof(icon), icon, null)
    };

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
