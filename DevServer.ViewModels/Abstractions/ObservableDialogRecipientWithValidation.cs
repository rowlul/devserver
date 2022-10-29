using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using HanumanInstitute.MvvmDialogs;

namespace DevServer.ViewModels;

public abstract partial class ObservableDialogRecipientWithValidation : ObservableValidator, IModalDialogViewModel,
    ICloseable
{
    private bool? _dialogResult;

    public event EventHandler? RequestClose;

    public bool? DialogResult
    {
        get => _dialogResult;
        protected set => SetProperty(ref _dialogResult, value);
    }

    protected ObservableDialogRecipientWithValidation()
        : this(WeakReferenceMessenger.Default)
    {
    }

    protected ObservableDialogRecipientWithValidation(IMessenger messenger)
    {
        Messenger = messenger;
    }

    protected IMessenger Messenger { get; }

    [RelayCommand]
    protected virtual void Close()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
