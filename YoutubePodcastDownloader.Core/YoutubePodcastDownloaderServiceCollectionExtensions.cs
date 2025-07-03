using Microsoft.Extensions.DependencyInjection;
using YoutubePodcastDownloader.Core.Services;

namespace YoutubePodcastDownloader.Core;

public static class YoutubePodcastDownloaderServiceCollectionExtensions
{
    public static IServiceCollection AddYoutubePodcastDownloader(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpClient<PlayerService>();

        return services;
    }
}
