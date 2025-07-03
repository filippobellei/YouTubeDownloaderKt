namespace YoutubePodcastDownloader.Core.Models.GetPlayerRequest;

public class Client
{
    public required string ClientName { get; set; }
    public required string ClientVersion { get; set; }
    public required string VisitorData { get; set; }
    public required string Hl { get; set; }
    public required string Gl { get; set; }
    public required int UtcOffsetMinutes { get; set; }
}
