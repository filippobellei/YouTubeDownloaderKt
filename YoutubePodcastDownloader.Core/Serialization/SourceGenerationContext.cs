using System.Text.Json.Serialization;
using YoutubePodcastDownloader.Core.Models.GetPlayerRequest;
using YoutubePodcastDownloader.Core.Models.GetPlayerResponse;

namespace YoutubePodcastDownloader.Core.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GetPlayerRequest))]
[JsonSerializable(typeof(GetPlayerResponse))]
public partial class SourceGenerationContext : JsonSerializerContext;
