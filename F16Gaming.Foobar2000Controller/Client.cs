using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using F16Gaming.Foobar2000Controller.Events;

namespace F16Gaming.Foobar2000Controller
{
	public class Client
	{
		public event MessageReceivedEventHandler MessageReceived;
		public event DisconnectEventHandler Disconnect;

		private TcpClient _tcpClient;
		private StreamReader _reader;
		private Thread _listenThread;

		public bool Active { get; private set; }

		internal Client(string host, ushort port)
		{
			Console.WriteLine("Creating client");

			_tcpClient = new TcpClient(host, port);
			_reader = new StreamReader(_tcpClient.GetStream());
			Active = true;
			_listenThread = new Thread(Listen) {Name = "ClientListen"};
			Console.WriteLine("Starting listen thread");
			_listenThread.Start();
		}

		private void OnMessageReceived(Message message)
		{
			var func = MessageReceived;
			if (func == null)
				return;

			func(this, new MessageReceivedEventArgs(message));
		}

		private void OnDisconnect()
		{
			var func = Disconnect;
			if (func == null)
				return;
			func(this, null);
		}

		public void Stop()
		{
			if (_reader == null)
				return;

			Console.WriteLine("Attempting to stop Client._listenThread...");
			_reader.Close(); // Will cause SocketException to be thrown in Listen(), stopping the thread
			_reader.Dispose();
			_reader = null;
		}

		private Message ParseMessage(string raw)
		{
			return new Message(raw);
		}

		private void Listen()
		{
			try
			{
				Console.WriteLine("Listen thread started, waiting for message from server");
				while (Active)
				{
					var msg = ParseMessage(_reader.ReadLine());
					if (msg.Type == MessageType.Play || msg.Type == MessageType.Pause || msg.Type == MessageType.Stop)
						msg = new SongMessage(msg);

					OnMessageReceived(msg);
				}
			}
			catch (ThreadAbortException)
			{
				Console.WriteLine("ThreadAbortException in Client.Listen(), Client._listenThread is stopping!");
			}
			catch (SocketException)
			{
				Console.WriteLine("SocketException while reading from server, Client._listenThread will terminate!");
			}
			catch (IOException)
			{
				Console.WriteLine("IOException while reading from server, Client._listenThread will terminate!");
			}
			catch (NullReferenceException)
			{
				Console.WriteLine("NullReferenceException while reading from server, Client._listenThread will terminate!");
			}
			finally
			{
				if (_reader != null)
					_reader.Close();
				if (_tcpClient != null)
					_tcpClient.Close();
				Active = false;
				Console.WriteLine("Client._listenThread has stopped");
				OnDisconnect();
			}
		}
	}
}
