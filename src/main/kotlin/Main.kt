import io.ktor.client.*
import io.ktor.client.call.*
import io.ktor.client.plugins.HttpTimeout
import io.ktor.client.request.*
import io.ktor.client.statement.bodyAsBytes
import io.ktor.utils.io.*
import io.ktor.utils.io.core.*
import kotlinx.coroutines.runBlocking
import kotlinx.io.asSink
import org.schabi.newpipe.extractor.NewPipe
import org.schabi.newpipe.extractor.services.youtube.YoutubeService
import org.schabi.newpipe.extractor.services.youtube.extractors.YoutubeStreamExtractor
import org.schabi.newpipe.extractor.services.youtube.linkHandler.YoutubeStreamLinkHandlerFactory
import java.io.File

fun main() {
    val downloader = DownloaderImpl()
    NewPipe.init(downloader)

    val youtubeStreamLinkHandlerFactory = YoutubeStreamLinkHandlerFactory.getInstance()
    val linkHandler = youtubeStreamLinkHandlerFactory.fromUrl("https://www.youtube.com/watch?v=mkggXE5e2yk")

    val youtubeService = YoutubeService(0)
    val youtubeStreamExtractor = YoutubeStreamExtractor(youtubeService, linkHandler)

    youtubeStreamExtractor.fetchPage()

    val contentUrl = youtubeStreamExtractor.videoStreams[0].content

    downloadVideo(contentUrl)
}

fun downloadVideo(url: String) {
    val client = HttpClient() {
        install(HttpTimeout) {
            requestTimeoutMillis = 200000
        }
    }

    val content = runBlocking {
        client.get(url).bodyAsBytes()
    }

    File("youtubeVideo.mp4").writeBytes(content)
}
