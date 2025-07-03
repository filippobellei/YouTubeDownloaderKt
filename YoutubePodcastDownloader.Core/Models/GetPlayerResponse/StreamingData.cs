namespace YoutubePodcastDownloader.Core.Models.GetPlayerResponse;

public class StreamingData
{
    public required IEnumerable<AdaptiveFormats> AdaptiveFormats { get; set; }
}
