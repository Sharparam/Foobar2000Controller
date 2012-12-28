using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;

namespace F16Gaming.Foobar2000Controller.WpfOverlay
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hwnd, int msg, int wparam, int lparam);

		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		private delegate void VoidDelegate();

		private const string SongFormat = @"~ {0} by {1} ({2}) ~";

		private const int WmNcLButtonDown = 0xA1;
		private const int HtCaption = 2;

		private readonly Controller _controller;

		private NotifyIcon _tray;

		private bool _clickThrough;
		private bool _ctToggling;

		public MainWindow()
		{
			InitializeComponent();

			try
			{
				_controller = new Controller("127.0.0.1", 3333);
				var client = _controller.GetClient();
				client.MessageReceived += (o, e) => UpdateStatus(e.Message);
				client.Disconnect += (o, e) => Disconnect();
				_tray = new NotifyIcon();
				_tray.Click += (o, e) => SetClickThrough(!_clickThrough);
				_tray.DoubleClick += (o, e) => Exit();
				_tray.Text = "Foobar2k Controller Overlay";
				Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Tray.ico")).Stream;
				_tray.Icon = new Icon(iconStream);
				_tray.Visible = true;
			}
			catch (Exception ex)
			{
				Background = Brushes.Red;
				StatusLabel.Content = "Failed to connect to control server (" + ex.GetType() + ").\nIs foobar2k running? Is foo_controlserver enabled?";
			}
		}

		private void Exit()
		{
			_controller.Stop();
			Close();
		}

		private void SetClickThrough(bool enabled)
		{
			IsHitTestVisible = !enabled;
			StatusLabel.IsHitTestVisible = !enabled;
			_clickThrough = enabled;
			Console.WriteLine(@"ClickThrough is now {0}", _clickThrough ? "enabled" : "diasbled");
			_ctToggling = false;
		}

		private void SetStatusText(string text)
		{
			if (StatusLabel.Dispatcher.CheckAccess())
			{
				StatusLabel.Content = text;
			}
			else
			{
				StatusLabel.Dispatcher.Invoke(DispatcherPriority.Normal, (VoidDelegate)(() => StatusLabel.Content = text));
			}
		}

		private void UpdateStatus(Message message)
		{
			var songMessage = message as SongMessage;
			if (songMessage == null || songMessage.Type != MessageType.Play)
			{
				switch (message.Type)
				{
					case MessageType.Pause:
						SetStatusText("Foobar2k is PAUSED");
						break;
					case MessageType.Stop:
						SetStatusText("Foobar2k is STOPPED");
						break;
				}
			}
			else
			{
				SetStatusText(string.Format(SongFormat, songMessage.Title, songMessage.Artist, songMessage.Album));
			}
		}

		private void Disconnect()
		{
			// Connection to foobar control server lost, close the application
			if (Dispatcher.CheckAccess())
				Close();
			else
				Dispatcher.Invoke((VoidDelegate) (Close));
		}

		private void MoveWindow()
		{
			if (_clickThrough)
				return;

			ReleaseCapture();
			SendMessage(new WindowInteropHelper(this).Handle, WmNcLButtonDown, HtCaption, 0);
		}

		private void StatusLabelMouseDown(object sender, MouseButtonEventArgs e)
		{
			MoveWindow();
		}

		private void WindowMouseDown1(object sender, MouseButtonEventArgs e)
		{
			MoveWindow();
		}
	}
}
