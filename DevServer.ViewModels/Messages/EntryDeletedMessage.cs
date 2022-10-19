using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DevServer.ViewModels.Messages;

public class EntryDeletedMessage : ValueChangedMessage<EntryViewModel>
{
    public EntryDeletedMessage(EntryViewModel value) : base(value)
    {
    }
}
