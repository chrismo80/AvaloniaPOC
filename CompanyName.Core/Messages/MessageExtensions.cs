using CompanyName.Core.Logging;

namespace CompanyName.Core.Messages;

public static class MessageExtensions
{
	static IMessageManager? _messageManager;

	/// <summary>
	/// Sets the Message extensions to use this IMessageManager for creating new messages via extensions
	/// </summary>
	/// <param name="messageManager">the IMessageManager to use</param>
	public static void ConfigureMessageExtensions(this IMessageManager messageManager) =>
		_messageManager = messageManager;

	public static void CreateMessage(this object me, string text, MessageType type = MessageType.Error)
	{
		var message = FormatMessage(me, text);

		_messageManager?.CreateMessage(message, type);

		me.Trace($"New {type} message: " + message, LogLevel.Debug);
	}

	public static void CreateMessage(this object me, Exception ex)
	{
		me.CreateMessage(ex.Message);
		me.Trace(ex);
	}

	private static string FormatMessage(this object me, string text) =>
		me.GetType().Name + ": " + text;
}