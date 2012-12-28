namespace F16Gaming.Foobar2000Controller.Console
{
	public class Program
	{
		private const string SongFormat = @"~ {0} by {1} ({2}) ~"; // <song> by <artist> (<album>)

		public static void Main(string[] args)
		{
			var controller = new Controller("127.0.0.1", 3333);
			var client = controller.GetClient();
			client.MessageReceived += (o, e) => MessageHandler(e.Message);
			System.Console.ReadLine();
			client.Stop();
			System.Console.ReadLine();
		}

		private static void MessageHandler(Message message)
		{
			var songMessage = message as SongMessage;
			if (songMessage != null)
			{
				SongMessageHandler(songMessage);
				return;
			}

			var parameters = message.GetParameters();
			if (message.Type == MessageType.Message)
				System.Console.WriteLine("Received message from server: {0}", parameters[0]);
		}

		private static void SongMessageHandler(SongMessage message)
		{
			System.Console.WriteLine(SongFormat, message.Title, message.Artist, message.Album);
		}
	}
}
