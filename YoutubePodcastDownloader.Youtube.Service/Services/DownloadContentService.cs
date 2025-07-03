namespace YoutubePodcastDownloader.Youtube.Service.Services;

public class RetrieveContentService(HttpClient _httpClient)
{
    private const long CHUNKSIZE = 1_000_000;

    public async Task<Stream> RetrieveContentStreamAsync(
        string baseUrl,
        long contentLength,
        CancellationToken cancellationToken = default
    )
    {
        var memoryStream = new MemoryStream();

        for (long i = 0; i < contentLength; i += CHUNKSIZE)
        {
            var url = baseUrl;
            url += $"&range={i}-{i + CHUNKSIZE - 1}";

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