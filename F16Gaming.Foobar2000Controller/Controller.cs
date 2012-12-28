using System;

namespace F16Gaming.Foobar2000Controller
{
	public class Controller
	{
		private readonly string _host;
		private readonly ushort _port;
		private readonly Client _client;

		public Controller(string host, ushort port)
		{
			_host = host;
			_port = port;

			_client = new Client(_host, _port);

			Console.WriteLine("Controller created");
		}

		public Client GetClient()
		{
			return _client;
		}

		public void Stop()
		{
			_client.Stop();
		}
	}
}
