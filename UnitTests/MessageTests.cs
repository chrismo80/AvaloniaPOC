using CompanyName.Core.Messages;

namespace UnitTests;

[TestClass]
public class MessageTests
{
	const int INITIAL_ACTIVE_MESSAGES = 10;
	const int INITIAL_ARCHIVED_MESSAGES = 0;

	MessageManager _sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		_sut = CreateSubjectUnderTest();
	}

	[TestCleanup]
	public void Cleanup()
	{
		_sut.Dispose();
	}

	[TestMethod]
	public void CreateMessage_NewMessage_Success()
	{
		_sut.CreateMessage("Message 11", MessageType.Information);

		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES + 1, _sut.ActiveMessages.Count);
	}

	[TestMethod]
	public void ArchiveMessage_LastActiveMessage_Success()
	{
		var message = _sut.ActiveMessages.Last();

		_sut.Archive(message);

		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, _sut.ActiveMessages.Count);
		Assert.AreEqual(INITIAL_ARCHIVED_MESSAGES + 1, _sut.ArchivedMessages.Count);

		Assert.AreEqual(message, _sut.ArchivedMessages.Last());
	}

	[TestMethod]
	public void CreateMessage_WithExtension_Success()
	{
		this.CreateMessage("Message", MessageType.Information);
		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, _sut.ActiveMessages.Count);

		_sut.ConfigureMessageExtensions();

		this.CreateMessage("Message", MessageType.Information);
		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES + 1, _sut.ActiveMessages.Count);
	}

	private static MessageManager CreateSubjectUnderTest()
	{
		var sut = new MessageManager();

		Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, sut.ActiveMessages.Count);
		Assert.AreEqual(INITIAL_ARCHIVED_MESSAGES, sut.ArchivedMessages.Count);

		return sut;
	}
}