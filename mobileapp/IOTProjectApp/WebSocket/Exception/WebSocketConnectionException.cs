namespace IOTProjectApp.WebSocket;

public class WebSocketConnectionException : Exception
{
    public WebSocketConnectionException(string message) : base(message)
    {
    }
}