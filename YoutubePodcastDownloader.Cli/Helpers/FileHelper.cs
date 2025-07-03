namespace YoutubePodcastDownloader.Cli.Helpers;

public static class FileHelper
{
    public static string SanitizeFileName(string unsafeFileName)
    {
        var fileName = unsafeFileName
            .Where(x => !Path.GetInvalidFileNameChars().Contains(x))
            .ToArray();

        return new string(fileName);
    }
}
