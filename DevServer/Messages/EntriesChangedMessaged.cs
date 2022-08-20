using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging.Messages;

using DevServer.ViewModels;

namespace DevServer.Messages;

public class EntriesChangedMessage : ValueChangedMessage<IList<EntryViewModel>>
{
    public EntriesChangedMessage(IList<EntryViewModel> value) : base(value)
    {
    }
}
