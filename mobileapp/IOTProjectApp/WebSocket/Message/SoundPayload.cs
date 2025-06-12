namespace IOTProjectApp.WebSocket.Message;

public class SoundPayload(byte[] data) : IReturnMessage
{
    // So bytes are immutable
    private readonly byte[] _data = data;
    public async Task<byte[]> GetMessageAsBytes()
    {
        return _data;
    }
}