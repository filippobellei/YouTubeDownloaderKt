using YoutubePodcastDownloader.Cli.Helpers;
using YoutubePodcastDownloader.Core.Services;

namespace YoutubePodcastDownloader.Cli;

public class HostedService(
    IConfiguration _configuration,
    PlayerService _player,
    ILogger<HostedService> _logger,
    IHostApplicationLifetime _lifetime
) : IHostedService
{
    private const string VIDEO_ID_CONFIGURATION = "VideoId";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var videoId = _configuration.GetValue<string>(VIDEO_ID_CONFIGURATION);
            ArgumentException.ThrowIfNullOrWhiteSpace(videoId);

            var player = await _player.GetPlayerAsync(videoId, cancellationToken);

            var audio = player.StreamingData.AdaptiveFormats.First();
            var url = audio.Url;
            var contentLength = Convert.ToInt64(audio.ContentLength);

            await using var stream = await _player.RetrieveContentStreamAsync(
                contentLength,
                url,
                cancellationToken
            );

            var fileName = FileHelper.SanitizeFileName(player.VideoDetails.Title) + ".opus";
            await using var fileStream = File.Create(fileName);

            await stream.CopyToAsync(fileStream, cancellationToken);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError("{Exception}", ex); }
        finally { _lifetime.StopApplication(); }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
