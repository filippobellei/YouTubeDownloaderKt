namespace YoutubePodcastDownloader.Core.Models.GetPlayerRequest;

public class GetPlayerRequest
{
    public required string VideoId { get; set; }
    public required Context Context { get; set; }
}
