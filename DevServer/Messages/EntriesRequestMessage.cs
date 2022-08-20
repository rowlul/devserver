using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging.Messages;

using DevServer.ViewModels;

namespace DevServer.Messages;

public class EntriesRequestMessage : AsyncRequestMessage<IList<EntryViewModel>>
{
}
