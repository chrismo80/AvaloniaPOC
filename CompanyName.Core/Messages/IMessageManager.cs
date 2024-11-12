namespace CompanyName.Core.Messages;

public interface IMessageManager
{
	event EventHandler<Message> Created;

	public void CreateMessage(string message, MessageType type = MessageType.Error);

	public void Confirm(Message message);

	public List<Message> ActiveMessages { get; }
}