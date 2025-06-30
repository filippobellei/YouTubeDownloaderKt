using YoutubePodcastDownloader.Youtube.Service.Contents;

namespace YoutubePodcastDownloader.Youtube.Service.Models.GetContentInfoResponse;

public class GetContentInfoResponse
{
    public PlayabilityStatus? PlayabilityStatus { get; set; }
    public StreamingData? StreamingData { get; set; }
    public VideoDetails? VideoDetails { get; set; }
}
