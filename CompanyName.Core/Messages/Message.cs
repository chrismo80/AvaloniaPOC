namespace CompanyName.Core.Messages;

public enum MessageType
{
	Information,
	Warning,
	Error
}

public record Message
{
	public DateTime Created = DateTime.Now;
	public int Id;
	public string Text = "";
	public MessageType Type;

	public override string ToString() => $"{Created}: {Text} ({Type})";
}