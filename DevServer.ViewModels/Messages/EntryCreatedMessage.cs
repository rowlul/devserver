using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DevServer.ViewModels.Messages;

public class EntryCreatedMessage : ValueChangedMessage<EntryViewModel>
{
    public EntryCreatedMessage(EntryViewModel value) : base(value)
    {
    }
}
