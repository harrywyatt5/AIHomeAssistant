namespace IOTProjectApp.WebSocket.Message;

public class EndRecordingAck : EndRecordingMessage, IReturnMessage
{
    public EndRecordingAck(int bytes)
    {
        BytesToSend = bytes;
        IsAck = true;
    }

    public int BytesToSend { get; private set; }
}