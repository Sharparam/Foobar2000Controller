using System.Globalization;

namespace F16Gaming.Foobar2000Controller
{
	public class SongMessage : Message
	{
		private const int PlaylistIdIndex = 0;
		private const int SongIdIndex = 1;
		private const int TimeIndex = 2;
		private const int ArtistIndex = 3;
		private const int AlbumIndex = 4;
		private const int SongNumberIndex = 5;
		private const int TitleIndex = 6;

		public readonly int PlaylistId;
		public readonly int SongId;
		public readonly float Time; // NOTE: CURRENT time on song, not duration of song
		public readonly string Artist;
		public readonly string Album;
		public readonly int SongNumber;
		public readonly string Title;

		internal SongMessage(string raw) : base(raw)
		{
			PlaylistId = int.Parse(Parameters[0].ToString());
			SongId = int.Parse(Parameters[1].ToString());
			Time = float.Parse(Parameters[2].ToString());
			Artist = Parameters[3].ToString();
			Album = Parameters[4].ToString();
			SongId = int.Parse(Parameters[5].ToString());
			Title = Parameters[6].ToString();
		}

		// Note: Possibly inefficient way to do it?
		internal SongMessage(Message baseMessage) : base(baseMessage.RawMessage)
		{
			// Parse all the different data
			PlaylistId = int.Parse(Parameters[PlaylistIdIndex].ToString());
			SongId = int.Parse(Parameters[SongIdIndex].ToString());
			// Workaround for the silly comma vs dot
			Time = float.Parse(Parameters[TimeIndex].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
			Artist = Parameters[ArtistIndex].ToString();
			Album = Parameters[AlbumIndex].ToString();
			SongNumber = int.Parse(Parameters[SongNumberIndex].ToString());
			Title = Parameters[TitleIndex].ToString();
		}
	}
}
