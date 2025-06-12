namespace IOTProjectApp.WebSocket.Message;

public class ServerReadyToReceive : BaseControlMessage
{
    public int BytesToReceive { get; private set; }
}