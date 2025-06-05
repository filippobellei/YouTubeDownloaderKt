import io.ktor.client.*
import io.ktor.client.request.request
import io.ktor.client.request.setBody
import io.ktor.client.statement.bodyAsText
import io.ktor.client.statement.request
import io.ktor.http.*
import io.ktor.util.toMap
import org.schabi.newpipe.extractor.downloader.*
import kotlinx.coroutines.runBlocking

class DownloaderImpl : Downloader() {
    private val client: HttpClient = HttpClient()

    override fun execute(request: Request): Response {
        val httpMethod = HttpMethod.parse(request.httpMethod())
        val url = request.url()
        val headers = request.headers()
        val requestBody = request.dataToSend()

        val response = runBlocking {
            client.request(url) {
                method = httpMethod
                headers {
                    headers.forEach { (headersName, headerValueList) ->
                        appendAll(headersName, headerValueList)
                    }
                }
                if (requestBody != null)
                    setBody(requestBody)
            }
        }

        val responseBodyToReturn = runBlocking {
            response.bodyAsText()
        }

        return Response(
            response.status.value,
            response.status.description,
            response.headers.toMap(),
            responseBodyToReturn,
            response.request.url.toString()
        )
    }
}
