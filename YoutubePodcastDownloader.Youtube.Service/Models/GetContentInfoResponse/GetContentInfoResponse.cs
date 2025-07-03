using YoutubePodcastDownloader.Youtube.Service.Contents;

namespace YoutubePodcastDownloader.Youtube.Service.Models.GetContentInfoResponse;

public class GetContentInfoResponse
{
    public required PlayabilityStatus PlayabilityStatus { get; set; }
    public required StreamingData StreamingData { get; set; }
    public required VideoDetails VideoDetails { get; set; }
}
