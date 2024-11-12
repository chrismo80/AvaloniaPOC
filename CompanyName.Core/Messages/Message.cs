namespace CompanyName.Core.Messages;

public enum MessageType
{
    Information,
    Warning,
    Error,
}

public class Message
{
    public DateTime Created = DateTime.Now;
    public MessageType Type;
    public string Text = "";
    public int Id;

    public override string ToString() => $"{Created}: {Text} ({Type})";
}