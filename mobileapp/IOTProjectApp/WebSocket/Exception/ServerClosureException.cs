using System.Net.WebSockets;

namespace IOTProjectApp.WebSocket;

public class ServerClosureException : Exception, IWebSocketException
{
    public ServerClosureException() : base("The server closed the connection")
    {
    }

    public WebSocketClosure GetClosure() => new(WebSocketCloseStatus.NormalClosure, "The server closed the connection");
}