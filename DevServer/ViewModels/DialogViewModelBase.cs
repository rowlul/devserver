using System;

using CommunityToolkit.Mvvm.Input;

using HanumanInstitute.MvvmDialogs;

namespace DevServer.ViewModels;

public partial class DialogViewModelBase : ViewModelBase, IModalDialogViewModel, ICloseable
{
    private bool? _dialogResult;

    public bool? DialogResult
    {
        get => _dialogResult;
        protected set => SetProperty(ref _dialogResult, value);
    }

    public event EventHandler? RequestClose;

    [RelayCommand]
    protected virtual void Close()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
