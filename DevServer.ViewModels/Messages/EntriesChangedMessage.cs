using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DevServer.ViewModels.Messages;

public class EntriesChangedMessage : ValueChangedMessage<IList<EntryViewModel>>
{
    public EntriesChangedMessage(IList<EntryViewModel> value) : base(value)
    {
    }
}
