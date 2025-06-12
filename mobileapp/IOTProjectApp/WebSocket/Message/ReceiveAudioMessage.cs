namespace IOTProjectApp.WebSocket.Message;

public class ReceiveAudioMessage : BaseControlMessage
{
    public int BytesToReceive { get; private set; }
}