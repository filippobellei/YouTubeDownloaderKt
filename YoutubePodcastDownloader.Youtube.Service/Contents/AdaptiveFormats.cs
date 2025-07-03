namespace YoutubePodcastDownloader.Youtube.Service.Contents;

public class AdaptiveFormats
{
    public required string Url { get; set; }
    public required string MimeType { get; set; }
    public required int Bitrate { get; set; }
    public required string ContentLength { get; set; }
    public bool IsDrc { get; set; }
}
