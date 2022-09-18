using CommunityToolkit.Mvvm.Messaging.Messages;

using DevServer.ViewModels;

namespace DevServer.Messages;

public class EntryDeletedMessage : ValueChangedMessage<EntryViewModel>
{
    public EntryDeletedMessage(EntryViewModel value) : base(value)
    {
    }
}
