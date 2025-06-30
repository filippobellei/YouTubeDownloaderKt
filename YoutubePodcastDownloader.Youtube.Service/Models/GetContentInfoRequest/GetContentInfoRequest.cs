namespace YoutubePodcastDownloader.Youtube.Service.Models.GetContentInfoRequest;

public class GetContentInfoRequest
{
    public required string VideoId { get; set; }
    public required Context Context { get; set; }
}
