namespace YoutubePodcastDownloader.Core.Models.GetPlayerResponse;

public class GetPlayerResponse
{
    public required PlayabilityStatus PlayabilityStatus { get; set; }
    public required StreamingData StreamingData { get; set; }
    public required VideoDetails VideoDetails { get; set; }
}
