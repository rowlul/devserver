using CommunityToolkit.Mvvm.Messaging.Messages;

using DevServer.ViewModels;

namespace DevServer.Messages;

public class EntryChangedMessage : ValueChangedMessage<EntryViewModel>
{
    public EntryChangedMessage(EntryViewModel value) : base(value)
    {
    }
}
