using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using HanumanInstitute.MvvmDialogs;

namespace DevServer.ViewModels;

public abstract partial class ObservableDialog : ObservableObject, IModalDialogViewModel, ICloseable
{
    private bool? _dialogResult;

    public event EventHandler? RequestClose;

    public bool? DialogResult
    {
        get => _dialogResult;
        protected set => SetProperty(ref _dialogResult, value);
    }

    [RelayCommand]
    protected virtual void Close()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
