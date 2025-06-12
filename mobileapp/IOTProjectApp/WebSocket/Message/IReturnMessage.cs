using System.Text.Json;
using System.Text;

namespace IOTProjectApp.WebSocket.Message;

// Return message can be both binary and text, unlike 
public interface IReturnMessage
{
    // Default implementation for messsages - binary messages must override
    public async Task<byte[]> GetMessageAsBytes()
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync<BaseControlMessage>(
            stream,
            (BaseControlMessage)this,
            JsonOptionsProvider.Singleton.GetGlobalOptions(),
            CancellationToken.None
        );

        return stream.ToArray();
    }
}