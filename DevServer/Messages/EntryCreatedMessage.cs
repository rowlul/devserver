using CommunityToolkit.Mvvm.Messaging.Messages;

using DevServer.ViewModels;

namespace DevServer.Messages;

public class EntryCreatedMessage : ValueChangedMessage<EntryViewModel>
{
    public EntryCreatedMessage(EntryViewModel value) : base(value)
    {
    }
}
