using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using YoutubePodcastDownloader.Youtube.Service.Models.GetContentInfoRequest;
using YoutubePodcastDownloader.Youtube.Service.Models.GetContentInfoResponse;

namespace YoutubePodcastDownloader.Youtube.Service.Services;

public class ContentInfoService(HttpClient _httpClient)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private async ValueTask<string> ResolveVisitorDataAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("https://www.youtube.com/sw.js_data", cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
        if (jsonString.StartsWith(")]}'"))
            jsonString = jsonString[4..];

        var json = JsonNode.Parse(jsonString);

        var visitorData = json?[0]?[2]?[0]?[0]?[13]?.ToString();

        if (string.IsNullOrWhiteSpace(visitorData))
            throw new Exception("Failed to resolve visitor data");

        return visitorData;
    }

    public async ValueTask<GetContentInfoResponse> GetContentInfoAsync(
        string videoId,
        CancellationToken cancellationToken = default
    )
    {
        var visitorData = await ResolveVisitorDataAsync(cancellationToken);

        var request = new GetContentInfoRequest
        {
            VideoId = videoId,
            Context = new Context
            {
                Client = new Client
                {
                    ClientName = "ANDROID",
                    ClientVersion = "20.25.35",
                    VisitorData = visitorData,
                    Hl = "en",
                    Gl = "US",
                    UtcOffsetMinutes = 0
                }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        using var requestContent = new StringContent(jsonRequest);

        using var response = await _httpClient.PostAsync(
            "https://www.youtube.com/youtubei/v1/player",
            requestContent,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<GetContentInfoResponse>(cancellationToken);

        var playabilityStatus = responseContent?.PlayabilityStatus?.Status;
        var details = responseContent?.VideoDetails;

        if (string.Equals(playabilityStatus, "error", StringComparison.OrdinalIgnoreCase) || details is null)
            throw new Exception($"Video '{videoId}' is not available");

        return responseContent!;
    }
}
