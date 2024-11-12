namespace CompanyName.Core.Messages;

public interface IMessageArchive
{
    public List<Message> ArchivedMessages { get; }

    public void Archive(Message message);
}