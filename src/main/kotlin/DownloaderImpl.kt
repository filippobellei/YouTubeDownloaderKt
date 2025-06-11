import okhttp3.OkHttpClient
import okhttp3.RequestBody
import okhttp3.RequestBody.Companion.toRequestBody
import org.schabi.newpipe.extractor.downloader.Downloader
import org.schabi.newpipe.extractor.downloader.Request
import org.schabi.newpipe.extractor.downloader.Response
import java.io.IOException

class DownloaderImpl : Downloader() {

    private val client = OkHttpClient()

    override fun execute(request: Request): Response {
        val requestBody: RequestBody? = request.dataToSend()?.toRequestBody()

        val requestBuilder = okhttp3.Request.Builder()
            .url(request.url())
            .method(request.httpMethod(), requestBody)

        request.headers().forEach { (name, values) ->
            requestBuilder.removeHeader(name)
            values.forEach { value ->
                requestBuilder.addHeader(name, value)
            }
        }

        client.newCall(requestBuilder.build()).execute().use { response ->
            if (!response.isSuccessful) {
                throw IOException("HTTP error code: ${response.code}")
            }

            val responseBody = response.body?.use { it.string() }.orEmpty()

            return Response(
                response.code,
                response.message,
                response.headers.toMultimap(),
                responseBody,
                response.request.url.toString()
            )
        }
    }
}
