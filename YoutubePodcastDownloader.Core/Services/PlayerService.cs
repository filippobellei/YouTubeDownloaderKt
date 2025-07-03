using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using YoutubePodcastDownloader.Core.Models.GetPlayerRequest;
using YoutubePodcastDownloader.Core.Models.GetPlayerResponse;
using YoutubePodcastDownloader.Core.Serialization;

namespace YoutubePodcastDownloader.Core.Services;

public class PlayerService(HttpClient _httpClient)
{
    public const string VISITOR_DATA_URL = "https://www.youtube.com/sw.js_data";
    public const string PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
    public const string CLIENT_NAME = "ANDROID";
    public const string CLIENT_VERSION = "20.25.35";
    public const string HL = "en";
    public const string GL = "US";
    public const string AUDIO_WEBM_CODEC = "audio/webm";
    private const int CHUNKSIZE = 1_000_000;

    private async Task<string> ResolveVisitorDataAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient
            .GetAsync(VISITOR_DATA_URL, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        if (content.StartsWith(")]}'"))
            content = content[4..];

        var json = JsonNode.Parse(content);

        var visitorData = json?[0]?[2]?[0]?[0]?[13]?.ToString();

        if (string.IsNullOrWhiteSpace(visitorData))
            throw new Exception("Failed to resolve visitor data");

        return visitorData;
    }

    public async Task<GetPlayerResponse> GetPlayerAsync(
        string videoId,
        CancellationToken cancellationToken = default
    )
    {
        var visitorData = await ResolveVisitorDataAsync(cancellationToken)
            .ConfigureAwait(false);

        var request = new GetPlayerRequest
        {
            VideoId = videoId,
            Context = new Context
            {
                Client = new Client
                {
                    ClientName = CLIENT_NAME,
                    ClientVersion = CLIENT_VERSION,
                    VisitorData = visitorData,
                    Hl = HL,
                    Gl = GL,
                    UtcOffsetMinutes = 0
                }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(
            request,
            SourceGenerationContext.Default.GetPlayerRequest
        );
        using var requestContent = new StringContent(jsonRequest);

        using var response = await _httpClient
            .PostAsync(
                PLAYER_URL,
                requestContent,
                cancellationToken
            )
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content
            .ReadFromJsonAsync(SourceGenerationContext.Default.GetPlayerResponse, cancellationToken)
            .ConfigureAwait(false);

        var playabilityStatus = responseContent?.PlayabilityStatus.Status;
        var details = responseContent?.VideoDetails;

        if (string.Equals(playabilityStatus, "error", StringComparison.OrdinalIgnoreCase) || details is null)
            throw new Exception($"Video '{videoId}' is not available");

        responseContent!.StreamingData.AdaptiveFormats = responseContent.StreamingData.AdaptiveFormats
            .Where(x => x.MimeType.StartsWith(AUDIO_WEBM_CODEC) && !x.IsDrc);

        return responseContent;
    }

    public async Task<Stream> RetrieveContentStreamAsync(
        long contentLength,
        string baseUrl,
        CancellationToken cancellationToken = default
    )
    {
        var memoryStream = new MemoryStream();

        for (long i = 0; i < contentLength; i += CHUNKSIZE)
        {
            var url = baseUrl + $"&range={i}-{i + CHUNKSIZE - 1}";

            using var response = await _httpClient
                .GetAsync(url, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            await responseContent
                .CopyToAsync(memoryStream, cancellationToken)
                .ConfigureAwait(false);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
