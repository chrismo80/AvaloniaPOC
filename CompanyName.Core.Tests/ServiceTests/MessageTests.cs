using CompanyName.Core.Messages;
using CompanyName.Core.Is;

namespace CompanyName.Core.Tests.ServiceTests;

[TestClass]
public class MessageTests : ServiceTests<MessageManager>
{
	[TestMethod]
	public void CreateMessage_NewMessage_Success()
	{
		Sut.CreateMessage("Message 1", MessageType.Information);

		Sut.ActiveMessages.Count.Is(1);
	}

	[TestMethod]
	public void ArchiveMessage_LastActiveMessage_Success()
	{
		Sut.CreateMessage("Message 1", MessageType.Information);
		Sut.CreateMessage("Message 2", MessageType.Information);

		Sut.ActiveMessages.Count.Is(2);
		Sut.ArchivedMessages.Count.Is(0);

		var message = Sut.ActiveMessages.Last();

		Sut.Archive(message);

		Sut.ActiveMessages.Count.Is(2);
		Sut.ArchivedMessages.Count.Is(1);

		Sut.ArchivedMessages[^1].Is(message);
	}

	[TestMethod]
	public void ConfirmMessage_AfterArchiveMessage_MessageMoved()
	{
		Sut.CreateMessage("Message 1", MessageType.Information);
		Sut.CreateMessage("Message 2", MessageType.Information);

		Sut.ActiveMessages.Count.Is(2);
		Sut.ArchivedMessages.Count.Is(0);

		var firstMessage = Sut.ActiveMessages[0];

		Sut.Archive(firstMessage);
		Sut.Confirm(firstMessage);

		Sut.ActiveMessages.Count.Is(1);
		Sut.ArchivedMessages.Count.Is(1);

		Sut.ArchivedMessages[^1].Is(firstMessage);
	}

	[TestMethod]
	public void CreateMessage_WithExtension_Success()
	{
		this.CreateMessage("Message", MessageType.Information);
		Sut.ActiveMessages.Count.Is(1);
	}

	[TestMethod]
	public void CreateMessage_ExceptionMessage_Success()
	{
		this.CreateMessage(new Exception("Test"));
		Sut.ActiveMessages.Count.Is(1);
	}

	protected override void Pre()
	{
		Sut.ActiveMessages.Count.Is(0);
		Sut.ArchivedMessages.Count.Is(0);

		// this enables message creation via object extension
		Sut.ConfigureMessageExtensions();
	}
}