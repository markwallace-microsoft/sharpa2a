using System.Net;
using System.Text.Json;

namespace SharpA2A.AspNetCore;

public class JsonRpcContent : HttpContent
{
    private readonly Stream stream;

    public JsonRpcContent(JsonRpcRequest request)
    {
        // Serialize the request to JSON and convert it to a byte array
        stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream);
        request.Write(writer);
        writer.Flush();
        stream.Position = 0;
    }
    public JsonRpcContent(JsonRpcResponse response)
    {
        Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        // Serialize the response to JSON and convert it to a byte array
        stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream);
        response.Write(writer);
        writer.Flush();
        stream.Position = 0;
    }

#if NET8_0_OR_GREATER
    protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        this.stream.CopyTo(stream);
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        await this.stream.CopyToAsync(stream, cancellationToken);
    }
#endif

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        this.stream.CopyTo(stream);
        return Task.CompletedTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        length = stream.Length;
        return true;
    }
}