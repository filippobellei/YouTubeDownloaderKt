using YoutubePodcastDownloader.Youtube.Service.Services;

namespace YoutubePodcastDownloader.Cli;

public class HostedService(
    ContentInfoService _contentInfoService,
    RetrieveContentService _downloadContentService,
    IHostApplicationLifetime _lifetime
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var contentInfo = await _contentInfoService.GetContentInfoAsync("QUfb4qw6euU", cancellationToken);

            var audio = contentInfo.StreamingData.AdaptiveFormats.First();

            var url = audio.Url;
            var contentLength = Convert.ToInt64(audio.ContentLength);
            var fileName = GetFileNameFromTitle(contentInfo.VideoDetails.Title);

            await using var stream = await _downloadContentService.RetrieveContentStreamAsync(url, contentLength, cancellationToken);
            await using var fileStream = File.Create(fileName);

            await stream.CopyToAsync(fileStream, cancellationToken);
        }
        catch (OperationCanceledException) { }
        finally { _lifetime.StopApplication(); }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static string GetFileNameFromTitle(string unsafeTitle)
    {
        unsafeTitle += ".opus";
        var title = unsafeTitle
            .Where(x => !Path.GetInvalidFileNameChars().Contains(x))
            .ToArray();

        return new string(title);
    }
}
