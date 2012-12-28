using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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

		private Controller _controller;

		public MainWindow()
		{
			InitializeComponent();

			_controller = new Controller("127.0.0.1", 3333);
			_controller.GetClient().MessageReceived += (o, e) => UpdateStatus(e.Message);
		}

		private void UpdateStatus(Message message)
		{
			var songMessage = message as SongMessage;
			if (songMessage == null)
				return;

			if (StatusLabel.Dispatcher.CheckAccess())
			{
				StatusLabel.Content = string.Format(SongFormat, songMessage.Title, songMessage.Artist, songMessage.Album);
			}
			else
			{
				StatusLabel.Dispatcher.Invoke(DispatcherPriority.Normal, (VoidDelegate)(() => StatusLabel.Content = string.Format(SongFormat, songMessage.Title, songMessage.Artist, songMessage.Album)));
			}
		}

		private void StatusLabelMouseDown(object sender, MouseButtonEventArgs e)
		{
			ReleaseCapture();
			SendMessage(new WindowInteropHelper(this).Handle, WmNcLButtonDown, HtCaption, 0);
		}
	}
}
