using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DevServer.ViewModels.Messages;

public sealed class ProcessRunningMessage : ValueChangedMessage<bool>
{
    public ProcessRunningMessage(bool value) : base(value)
    {
    }
}
