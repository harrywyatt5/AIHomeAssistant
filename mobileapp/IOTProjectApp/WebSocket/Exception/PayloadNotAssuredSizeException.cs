using System.Net.WebSockets;

namespace IOTProjectApp.WebSocket;

public class PayloadNotAssuredSizeException : Exception, IWebSocketException
{
    public PayloadNotAssuredSizeException(int actualSize, int assuredSize) 
        : base($"Payload expected to be {assuredSize}, received {actualSize}")
    {
    }

    public WebSocketClosure GetClosure() => new(WebSocketCloseStatus.InvalidPayloadData, Message);
}