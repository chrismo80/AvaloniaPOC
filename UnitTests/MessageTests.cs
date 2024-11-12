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

        Assert.AreEqual(INITIAL_ACTIVE_MESSAGES + 1, _sut.GetActiveMessages().Count);
    }

    [TestMethod]
    public void ArchiveMessage_LastActiveMessage_Success()
    {
        var message = _sut.GetActiveMessages().Last();

        _sut.Archive(message);

        Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, _sut.GetActiveMessages().Count);
        Assert.AreEqual(INITIAL_ARCHIVED_MESSAGES + 1, _sut.GetArchivedMessages().Count);

        Assert.AreEqual(message, _sut.GetArchivedMessages().Last());
    }

    [TestMethod]
    public void CreateMessage_WithExtension_Success()
    {
        this.CreateMessage("Message", MessageType.Information);
        Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, _sut.GetActiveMessages().Count);

        _sut.ConfigureMessageExtensions();

        this.CreateMessage("Message", MessageType.Information);
        Assert.AreEqual(INITIAL_ACTIVE_MESSAGES + 1, _sut.GetActiveMessages().Count);
    }

    private static MessageManager CreateSubjectUnderTest()
    {
        var sut = new MessageManager();

        Assert.AreEqual(INITIAL_ACTIVE_MESSAGES, sut.GetActiveMessages().Count);
        Assert.AreEqual(INITIAL_ARCHIVED_MESSAGES, sut.GetArchivedMessages().Count);

        return sut;
    }
}