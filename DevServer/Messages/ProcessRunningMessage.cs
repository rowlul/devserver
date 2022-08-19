using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DevServer.Messages;

public sealed class ProcessRunningMessage : ValueChangedMessage<bool>
{
    public ProcessRunningMessage(bool value) : base(value)
    {
    }
}
