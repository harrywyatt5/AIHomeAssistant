namespace IOTProjectApp.WebSocket;

public class InvalidMessageException : Exception
{
    public InvalidMessageException(Type receivedType) 
        : base(receivedType + " is not valid for this state")
    {
    }
}