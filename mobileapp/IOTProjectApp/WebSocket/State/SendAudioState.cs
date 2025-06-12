using System.Net.WebSockets;
using Android.Util;
using IOTProjectApp.WebSocket.Message;

namespace IOTProjectApp.WebSocket.State;

public class SendAudioState(byte[] data) : IState
{
    private readonly byte[] _audioData = data;
    
    public async Task<ResponseContext> ReceiveMessageInState(PayloadWrapper payload, CancellationToken cTk)
    {
        // Has to be ready to receive as only form accepted
        await payload.GetAsControlMessage([typeof(ServerReadyToReceive)], cTk);

        Log.Debug("DEBUG", $"Current payload size {_audioData.Length}");
        
        // Will only run if correct message is passed
        return new ResponseContext(
            new ReceivingAudioState(), 
            new SoundPayload(_audioData),
            WebSocketMessageType.Binary
        );
    }
}