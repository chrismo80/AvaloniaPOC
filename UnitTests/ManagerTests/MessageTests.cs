using CompanyName.Core.Messages;

namespace UnitTests.ManagerTests;

[TestClass]
public class MessageTests : ManagerTests<MessageManager>
{
	protected override void VerifyInitialState()
	{
		Assert.AreEqual(0, Sut.ActiveMessages.Count);
		Assert.AreEqual(0, Sut.ArchivedMessages.Count);
	}

	[TestMethod]
	public void CreateMessage_NewMessage_Success()
	{
		Sut.CreateMessage("Message 1", MessageType.Information);

		Assert.AreEqual(1, Sut.ActiveMessages.Count);
	}

	[TestMethod]
	public void ArchiveMessage_LastActiveMessage_Success()
	{
		Sut.CreateMessage("Message 1", MessageType.Information);
		Sut.CreateMessage("Message 2", MessageType.Information);

		Assert.AreEqual(2, Sut.ActiveMessages.Count);
		Assert.AreEqual(0, Sut.ArchivedMessages.Count);

		var message = Sut.ActiveMessages.Last();

		Sut.Archive(message);

		Assert.AreEqual(2, Sut.ActiveMessages.Count);
		Assert.AreEqual(1, Sut.ArchivedMessages.Count);

		Assert.AreEqual(message, Sut.ArchivedMessages.Last());
	}

	[TestMethod]
	public void ConfirmMessage_AfterArchiveMessage_MessageMoved()
	{
		Sut.CreateMessage("Message 1", MessageType.Information);
		Sut.CreateMessage("Message 2", MessageType.Information);

		Assert.AreEqual(2, Sut.ActiveMessages.Count);
		Assert.AreEqual(0, Sut.ArchivedMessages.Count);

		var firstMessage = Sut.ActiveMessages.First();

		Sut.Archive(firstMessage);
		Sut.Confirm(firstMessage);

		Assert.AreEqual(1, Sut.ActiveMessages.Count);
		Assert.AreEqual(1, Sut.ArchivedMessages.Count);

		Assert.AreEqual(firstMessage, Sut.ArchivedMessages.Last());
	}

	[TestMethod]
	public void CreateMessage_WithExtension_Success()
	{
		this.CreateMessage("Message", MessageType.Information);
		Assert.AreEqual(0, Sut.ActiveMessages.Count);

		// this enables message creation via object extension
		Sut.ConfigureMessageExtensions();

		this.CreateMessage("Message", MessageType.Information);
		Assert.AreEqual(1, Sut.ActiveMessages.Count);
	}
}