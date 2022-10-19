using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DevServer.ViewModels.Messages;

public class EntriesRequestMessage : AsyncRequestMessage<IList<EntryViewModel>>
{
}
