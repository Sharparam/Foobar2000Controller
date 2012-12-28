namespace F16Gaming.Foobar2000Controller.Events
{
	public class MessageReceivedEventArgs
	{
		public Message Message { get; private set; }

		internal MessageReceivedEventArgs(Message message)
		{
			Message = message;
		}
	}

	public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
}
