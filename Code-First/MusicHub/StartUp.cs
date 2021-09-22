namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albums = context.Albums
                .Where(x => x.ProducerId == producerId)
                .Select(x => new
                {
                    x.Name,
                    ReleaseDate = x.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = x.Producer.Name,
                    AlbumSongs = x.Songs
                        .Select(song => new
                        {
                            SongName = song.Name,
                            SongPrice = song.Price,
                            SongWriterName = song.Writer.Name
                        })
                        .OrderByDescending(x => x.SongName)
                        .ThenBy(x => x.SongWriterName)
                        .ToList(),
                    TotalAlbumPrice = x.Price
                })
                .ToList()
                .OrderByDescending(x => x.TotalAlbumPrice);

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int counter = 0;

                foreach (var song in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{++counter}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:0.00}");
                    sb.AppendLine($"---Writer: {song.SongWriterName}");
                }
                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:0.00}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .Select(x => new
                {
                    x.Name,
                    PerformerFullName = x.SongPerformers.FirstOrDefault().Performer.FirstName + " " + x.SongPerformers.FirstOrDefault().Performer.LastName,
                    WriterName = x.Writer.Name,
                    AlbumProducer = x.Album.Producer,
                    x.Duration
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.WriterName)
                .ThenBy(x => x.PerformerFullName)
                .ToList()
                .Where(x => x.Duration.TotalSeconds > (double)duration);

            int counter = 0;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{++counter}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");
                string performerFullName = song.PerformerFullName == " " ? "" : song.PerformerFullName;
                sb.AppendLine($"---Performer: {performerFullName}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer.Name}");
                sb.AppendLine($"---Duration: {song.Duration.ToString("c")}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
