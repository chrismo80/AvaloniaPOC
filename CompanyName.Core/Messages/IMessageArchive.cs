namespace CompanyName.Core.Messages;

public interface IMessageArchive
{
    public List<Message> GetArchivedMessages();

    public void Archive(Message message);
}