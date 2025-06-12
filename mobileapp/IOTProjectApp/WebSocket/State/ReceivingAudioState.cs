using System.Net.WebSockets;
using IOTProjectApp.WebSocket.Message;

namespace IOTProjectApp.WebSocket.State;

public class ReceivingAudioState : IState
{
    private int _bytesToReceive;

    public ReceivingAudioState()
    {
        // This is an invalid number so we can check if number of bytes has been received
        _bytesToReceive = -1;
    }

    private async Task<ResponseContext> _handleIncomingBinary(ReadOnlyMemory<byte> binaryData, CancellationToken cTk)
    {
        // Create audio
        var audio = await new Audio.Builder()
            .WithAudioData(binaryData)
            .BuildAsync(cTk);
        
        // Attempt play and capture audio object
        audio.Play();
        AudioManager.Singleton.AddAudio(audio);
        
        // Finally, send a transaction complete message and return to idle state
        return new ResponseContext(
            new IdleState(),
            new TransactionCompleteMessage(),
            WebSocketMessageType.Text
        );
    }

    public async Task<ResponseContext> ReceiveMessageInState(PayloadWrapper wrappedPayload, CancellationToken cTk)
    {
        // If this is true, then we can handle the binary data
        if (_bytesToReceive >= 0) 
            return await _handleIncomingBinary(wrappedPayload.GetBinaryMessage(), cTk);
        
        // We can force upcast because we can be sure that it is this type
        var message = (ReceiveAudioMessage)await wrappedPayload.GetAsControlMessage([typeof(ReceiveAudioMessage)], cTk);
        _bytesToReceive = message.BytesToReceive;

        return new ResponseContext(this, new ReceiveAudioMessageAck(), WebSocketMessageType.Text);

    }
}