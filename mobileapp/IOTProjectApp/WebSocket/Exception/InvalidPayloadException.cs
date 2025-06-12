using System.Net.WebSockets;

namespace IOTProjectApp.WebSocket;

public class InvalidPayloadException : Exception, IWebSocketException
{
    public InvalidPayloadException(string received, string needed) 
        : base($"Expected a {needed} payload. Got {received}")
    {
    }

    public WebSocketClosure GetClosure() => new(WebSocketCloseStatus.InvalidPayloadData, this.Message);
}