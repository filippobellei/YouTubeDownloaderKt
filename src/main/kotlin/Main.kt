import okhttp3.OkHttpClient
import okhttp3.Request
import org.schabi.newpipe.extractor.NewPipe
import org.schabi.newpipe.extractor.services.youtube.YoutubeService
import org.schabi.newpipe.extractor.services.youtube.extractors.YoutubeStreamExtractor
import org.schabi.newpipe.extractor.services.youtube.linkHandler.YoutubeStreamLinkHandlerFactory
import java.io.*

fun main() {
    NewPipe.init(DownloaderImpl())

    val videoUrl = "https://www.youtube.com/watch?v=mkggXE5e2yk"
    val videoStreamUrl = extractVideoStreamUrl(videoUrl)

    if (videoStreamUrl != null)
        downloadVideo(videoStreamUrl)
}

fun extractVideoStreamUrl(youtubeUrl: String): String? {
    val linkHandler = YoutubeStreamLinkHandlerFactory
        .getInstance()
        .fromUrl(youtubeUrl)

    val youtubeService = YoutubeService(0)
    val extractor = YoutubeStreamExtractor(youtubeService, linkHandler)

    extractor.fetchPage()

    return extractor.videoStreams.firstOrNull()?.content
}

fun downloadVideo(url: String) {
    val client = OkHttpClient()
    val request = Request.Builder()
        .url(url)
        .build()

    client.newCall(request).execute().use { response ->
        if (!response.isSuccessful) {
            throw IOException("HTTP error code: ${response.code}")
        }

        response.body?.byteStream()?.use { inputStream ->
            File("youtubeVideo.mp4").outputStream().use { outputStream ->
                inputStream.copyTo(outputStream)
            }
        } ?: throw IOException("Response body is null")
    }
}
