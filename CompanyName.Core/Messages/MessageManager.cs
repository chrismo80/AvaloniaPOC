﻿namespace CompanyName.Core.Messages;

public class MessageManager : Manager, IMessageManager, IMessageArchive
{
	public List<Message> ActiveMessages { get; } = [];
	public List<Message> ArchivedMessages { get; } = [];

	public event EventHandler<Message>? Created;

	public MessageManager()
	{
	}

	public void CreateMessage(string text, MessageType type = MessageType.Error)
	{
		var message = new Message() { Text = text, Id = 0, Type = type };

		ActiveMessages.Add(message);

		Created?.Invoke(this, message);
	}

	public void Confirm(Message message) => ActiveMessages.Remove(message);

	public void Archive(Message message) => ArchivedMessages.Add(message);
}