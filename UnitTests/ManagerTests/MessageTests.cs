using CompanyName.Core.Messages;

namespace UnitTests.ManagerTests;

[TestClass]
public class MessageTests : ManagerTests<MessageManager>
{
	const int INITIAL_ACTIVE_MESSAGES = 10;
	const int INITIAL_ARCHIVED_MESSAGES = 0;

	protected override void InitialAsserts()
	{
		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, Sut.ActiveMessages.Count);
		Assert.AreEqual(INITIAL_ARCHIVED_MESSAGES, Sut.ArchivedMessages.Count);
	}

	[TestMethod]
	public void CreateMessage_NewMessage_Success()
	{
		Sut.CreateMessage("Message 11", MessageType.Information);

		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES + 1, Sut.ActiveMessages.Count);
	}

	[TestMethod]
	public void ArchiveMessage_LastActiveMessage_Success()
	{
		var message = Sut.ActiveMessages.Last();

		Sut.Archive(message);

		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, Sut.ActiveMessages.Count);
		Assert.AreEqual(INITIAL_ARCHIVED_MESSAGES + 1, Sut.ArchivedMessages.Count);

		Assert.AreEqual(message, Sut.ArchivedMessages.Last());
	}

	[TestMethod]
	public void CreateMessage_WithExtension_Success()
	{
		this.CreateMessage("Message", MessageType.Information);
		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, Sut.ActiveMessages.Count);

		Sut.ConfigureMessageExtensions();

		this.CreateMessage("Message", MessageType.Information);
		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES + 1, Sut.ActiveMessages.Count);
	}
}