using System.Net.WebSockets;
using System.Text.Json;
using Android.Util;
using IOTProjectApp.WebSocket.Message;
using Java.Lang;

namespace IOTProjectApp.WebSocket;

public class PayloadWrapper
{
    private MemoryStream _payload;
    private JsonSerializerOptions _options;
    private WebSocketMessageType _type;

    public PayloadWrapper(MemoryStream ms, WebSocketMessageType type)
    {
        _payload = ms;
        _options = JsonOptionsProvider.Singleton.GetGlobalOptions();
        _type = type;
    }

    public async Task<BaseControlMessage> GetAsControlMessage(Type[] allowedMessage, CancellationToken cTk)
    {
        // If not control message
        if (_type != WebSocketMessageType.Text) throw new InvalidPayloadException(_type.ToString(), "text");
        
        BaseControlMessage? deserializedMessage = await JsonSerializer.DeserializeAsync<BaseControlMessage>(_payload, _options, cTk);

        if (deserializedMessage is null) throw new NullPointerException("The message received was null");
        // See if we are allowed to use 
        for (int i = 0; i < allowedMessage.Length; ++i)
        {
            if (deserializedMessage.GetType() == allowedMessage[i]) return deserializedMessage;
        }

        throw new InvalidMessageException(deserializedMessage.GetType());
    }

    public ReadOnlyMemory<byte> GetBinaryMessage()
    {
        if (_type != WebSocketMessageType.Binary) throw new InvalidPayloadException(_type.ToString(), "binary");

        return _payload.ToArray();
    }

    public long GetSize() => _payload.Length;
}