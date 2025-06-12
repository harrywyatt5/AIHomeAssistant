using System.Net.WebSockets;

namespace IOTProjectApp.WebSocket;

public class PayloadTooBigException : Exception, IWebSocketException
{
    public PayloadTooBigException() : base("Payload must be under 10mb")
    {
    }

    public WebSocketClosure GetClosure() => new(WebSocketCloseStatus.InvalidPayloadData, "Overall payload must be under 10mb");
}