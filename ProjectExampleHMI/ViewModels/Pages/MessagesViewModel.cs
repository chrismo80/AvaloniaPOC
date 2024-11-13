using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using CompanyName.Core.Messages;

namespace ProjectExampleHMI.ViewModels;

public partial class MessagesViewModel : CompanyName.UI.ViewModels.PageViewModel
{
	readonly IMessageManager? _messageManager;
	readonly IMessageArchive? _messageArchive;

	[ObservableProperty]
	ObservableCollection<ObservableMessage> _activeMessages = [];

	[ObservableProperty]
	int _selectedIndex;

	[ObservableProperty]
	ObservableCollection<ObservableMessage> _archivedMessages = [];

	[ObservableProperty]
	int _selectedArchivedIndex;

	[ObservableProperty]
	string _message = "This is the Messages ViewModel";

	public MessagesViewModel()
	{
		Name = "Messages";

		ActiveMessages = new ObservableCollection<ObservableMessage>(Enumerable.Range(1, 3)
			.Select(i => new ObservableMessage(new Message() { Id = i, Text = "Dummy", Type = MessageType.Warning })));
	}

	public MessagesViewModel(IMessageManager messageManager, IMessageArchive messageArchive)
		: this()
	{
		_messageManager = messageManager;
		_messageArchive = messageArchive;

		CreateDummyMessages(10);

		ReadActiveMessages();

		_messageManager.Created += (_, _) => ReadActiveMessages();
	}

	public void Confirm()
	{
		if (SelectedIndex < 0)
			return;

		var selected = SelectedIndex;

		var message = ActiveMessages[SelectedIndex].Message;

		_messageManager?.Confirm(message);
		_messageArchive?.Archive(message);

		ReadActiveMessages();
		ReadArchivedMessages();

		Message = "Message: '" + message + "' confirmed!";

		SelectedIndex = Math.Min(selected, ActiveMessages.Count - 1);
	}

	private void ReadActiveMessages()
	{
		ActiveMessages = new ObservableCollection<ObservableMessage>(_messageManager?.ActiveMessages.Select(m => new ObservableMessage(m)) ?? []);
	}

	private void ReadArchivedMessages()
	{
		ArchivedMessages = new ObservableCollection<ObservableMessage>(_messageArchive?.ArchivedMessages.Select(m => new ObservableMessage(m)) ?? []);
	}

	private void CreateDummyMessages(int count)
	{
		for (var i = 1; i <= count; i++)
			_messageManager?.CreateMessage($"Dummy Message {i}", MessageType.Information);
	}
}

public partial class ObservableMessage : ObservableObject
{
	[ObservableProperty]
	private DateTime _created;

	[ObservableProperty]
	private string _text = "";

	[ObservableProperty]
	private int _id;

	[ObservableProperty]
	private string _type;

	public Message Message { get; }

	public ObservableMessage(Message message)
	{
		Created = message.Created;
		Message = message;
		Text = message.Text;
		Id = message.Id;
		this.Type = message.Type.ToString();
	}
}