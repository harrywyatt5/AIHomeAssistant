namespace IOTProjectApp.WebSocket.Message;

public class RecordMessage : BaseControlMessage, IReturnMessage
{
    public RecordMessage(bool isAck)
    {
        this.IsAck = isAck;
    }
}