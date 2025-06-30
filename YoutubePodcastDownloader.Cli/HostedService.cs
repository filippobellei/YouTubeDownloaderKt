using System.Diagnostics;
using YoutubePodcastDownloader.Youtube.Service.Services;

namespace YoutubePodcastDownloader.Cli;

public class HostedService(
    ContentInfoService _youtubeService,
    HttpClient _client,
    ILogger<HostedService> _logger,
    IHostApplicationLifetime _lifetime
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var getVideoResponse = await _youtubeService.GetContentInfoAsync("mkggXE5e2yk", cancellationToken);

            var audios = getVideoResponse?.StreamingData?.AdaptiveFormats?
                .Where(x => x!.MimeType!.StartsWith("audio/webm") && !x.IsDrc);
            var url = audios?.FirstOrDefault()?.Url;

            var unsafeTitle = getVideoResponse?.VideoDetails?.Title;
            var title = unsafeTitle?
                .Where(x => !Path.GetInvalidFileNameChars().Contains(x))
                .ToArray();
            var fileName = new string(title) + ".opus";

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            using var response = await _client.GetAsync(url, cancellationToken);
            stopWatch.Stop();
            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(fileName, FileMode.Create);

            await responseContent.CopyToAsync(fileStream, cancellationToken);
            await fileStream.FlushAsync(cancellationToken);

            _logger.LogInformation("Downloaded in {ElapsedTime}", stopWatch.Elapsed);
        }
        catch (OperationCanceledException) { }
        finally { _lifetime.StopApplication(); }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
