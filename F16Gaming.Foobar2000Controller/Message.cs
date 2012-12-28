namespace F16Gaming.Foobar2000Controller
{
	public class Message
	{
		public const char Delimiter = '|';

		public readonly string RawMessage;
		public readonly MessageType Type;
		
		protected readonly object[] Parameters;

		internal Message(string raw)
		{
			// Prepare message for parsing
			RawMessage = raw.TrimEnd(Delimiter);

			// Split it in parse based on delimiter
			string[] parts = RawMessage.Split(Delimiter);

			// Get the message type, always the first parameter and always present
			Type = (MessageType) int.Parse(parts[0]);

			// There is always at least one additional parameter, create array to hold extra parameters
			Parameters = new object[parts.Length - 1];

			// Read in the parameters from the previously split message
			for (int i = 1; i < parts.Length; i++)
				Parameters[i - 1] = parts[i];
		}

		public object[] GetParameters()
		{
			return Parameters;
		}
	}
}
