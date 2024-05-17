namespace MusicHub;

using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Data;
using Initializer;
using Microsoft.EntityFrameworkCore;

public class StartUp
{
    public static void Main()
    {
        MusicHubDbContext context =
            new MusicHubDbContext();

        //DbInitializer.ResetDatabase(context);

        using (context)
        {
            var songs = context.Songs
                .Where(s => s.Id == 3)
                .ToList();

            XDocument doc = new XDocument();
            XElement root = new("Songs");

            foreach (var song in songs)
            {
                var songX = new XElement("song");
                songX.Add(
                        new XElement("Name", song.Name),
                        new XElement("Price", $"{song.Price:f2}"),
                        new XElement("AlbumName", song.Album.Name));

                root.Add(songX);
            }

            doc.Add(root);
            doc.Save("products.xml");
        }

        //string result = ExportSongsAboveDuration(context, 4);
        //Console.WriteLine(result);
    }

    public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
    {
        //Formattingof numbers int the .Select() gives you data ready to fill any ViewModel
        //This ViewModel can be passed to any View

        var albums = context.Albums
            .Where(a => a.ProducerId == producerId)
            .Select(a => new
            {
                AlbumName = a.Name,
                ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                ProducerName = a.Producer.Name,
                Songs = a.Songs
                    .Select(s => new
                    {
                        SongName = s.Name,
                        SongPrice = s.Price.ToString("f2"),
                        WriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.WriterName)
                    .ToList(),
                TotalAlbumPrice = a.Songs.Sum(s => s.Price).ToString("f2")

            })
            .OrderByDescending(ap => ap.TotalAlbumPrice)
            .ToList();

        StringBuilder sb = new StringBuilder();
        foreach (var album in albums)
        {
            sb.AppendLine($"-AlbumName: {album.AlbumName}");
            sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
            sb.AppendLine($"-ProducerName: {album.ProducerName}");
            sb.AppendLine("-Songs:");

            int songNum = 1;
            foreach (var song in album.Songs)
            {
                sb.AppendLine($"---#{songNum++}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Price: {song.SongPrice:f2}");
                sb.AppendLine($"---Writer: {song.WriterName}");
            }

            sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
    {
        var songs = context.Songs
            .AsEnumerable()
            .Where(s => s.Duration.TotalSeconds > duration)
            .Select(s => new
            {
                SongName = s.Name,
                WriterName = s.Writer.Name,
                Performers = s.SongPerformers
                    .Select(p => new
                    {
                        PerformerFullName = $"{p.Performer.FirstName} {p.Performer.LastName}"
                    })
                    .OrderBy(p => p.PerformerFullName)
                    .ToList(),
                AlbumProducer = s.Album.Producer.Name,
                Duration = s.Duration.ToString("c")
            })
            .OrderBy(s => s.SongName)
            .ThenBy(s => s.WriterName)
            .ToList();

        StringBuilder sb = new StringBuilder();
        int counter = 1;

        foreach (var song in songs)
        {
            sb.AppendLine($"-Song #{counter}");
            sb.AppendLine($"---SongName: {song.SongName}");
            sb.AppendLine($"---Writer: {song.WriterName}");

            if (song.Performers.Any())
            {
                foreach (var performer in song.Performers)
                {
                    sb.AppendLine($"---Performer: {performer.PerformerFullName}");
                }
            }

            sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
            sb.AppendLine($"---Duration: {song.Duration}");

            counter++;
        }

        return sb.ToString().TrimEnd();
    }
}
