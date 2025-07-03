using YoutubePodcastDownloader.Cli;
using YoutubePodcastDownloader.Core;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddYoutubePodcastDownloader();
builder.Services.AddHostedService<HostedService>();

var host = builder.Build();

host.Run();
