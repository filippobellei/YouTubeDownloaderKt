using YoutubePodcastDownloader.Cli;
using YoutubePodcastDownloader.Youtube.Service.Services;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ContentInfoService>();
builder.Services.AddHostedService<HostedService>();

var host = builder.Build();

host.Run();
