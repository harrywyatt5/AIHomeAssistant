using System.Net.WebSockets;

namespace IOTProjectApp.WebSocket;

public class WebSocketClosure
{
    public WebSocketCloseStatus Status { get; private set; }
    public string CloseReason { get; private set; }

    public WebSocketClosure(WebSocketCloseStatus status, string reason)
    {
        Status = status;
        CloseReason = reason;
    }
}