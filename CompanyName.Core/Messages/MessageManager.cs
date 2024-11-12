namespace CompanyName.Core.Messages;

public class MessageManager : Manager, IMessageManager, IMessageArchive
{
    readonly List<Message> _activeMessages = [];

    readonly List<Message> _archivedMessages = [];

    public event EventHandler<Message>? Created;

    public MessageManager()
    {
        _activeMessages = Enumerable.Range(1, 10)
            .Select(i => new Message() { Text = $"Text {i}", Id = i, Type = MessageType.Warning })
            .ToList();
    }

    public void CreateMessage(string text, MessageType type = MessageType.Error)
    {
        var message = new Message() { Text = text, Id = 0, Type = type };

        _activeMessages.Add(message);

        Created?.Invoke(this, message);
    }

    public void Confirm(Message message) =>
        _activeMessages.Remove(message);

    public List<Message> GetActiveMessages() =>
        _activeMessages;

    public List<Message> GetArchivedMessages() =>
        _archivedMessages;

    public void Archive(Message message) =>
        _archivedMessages.Add(message);
}