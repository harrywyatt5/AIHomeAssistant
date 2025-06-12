using System.Net.WebSockets;
using Android.Util;
using IOTProjectApp.WebSocket.Message;

namespace IOTProjectApp.WebSocket.State;

public class IdleState : IState
{
    private Recorder? _currentRecorder;

    private ResponseContext _handleRecordMessage(CancellationToken cTk)
    {
        _currentRecorder = new Recorder();
        Log.Debug("DEBUG", "Recorder created");
        _currentRecorder.Prepare(cTk);
        Log.Debug("DEBUG", "Recorder has been prepared");
        _currentRecorder.StartRecording();
            
        // Return ACK to say we have 
        return new ResponseContext(
            this, 
            new RecordMessage(true),
            WebSocketMessageType.Text
        );
    }

    private async Task<ResponseContext> _handleEndRecording(CancellationToken cTk)
    {
        // throw if we haven't made a recorder yet
        if (_currentRecorder is null) throw new InvalidMessageException(typeof(EndRecordingMessage));
        
        // Stop recorder and get all bytes from file
        _currentRecorder.StopRecording();
        var audioBytes = await _currentRecorder.ReadRecordingAsync(cTk);
        
        // Then release the recorder
        _currentRecorder.Dispose();
        
        // and send an ack with how many bytes we want to send them
        return new ResponseContext(
            new SendAudioState(audioBytes),
            new EndRecordingAck(audioBytes.Length),
            WebSocketMessageType.Text
        );
    }

    public async Task<ResponseContext> ReceiveMessageInState(PayloadWrapper payload, CancellationToken cTk)
    {
        Log.Debug("DEBUG", "start idle state logic");
        // Get message
        BaseControlMessage message = await payload.GetAsControlMessage(
            [typeof(RecordMessage), typeof(EndRecordingMessage)],
            cTk
        );

        switch (message)
        {
            // Take appropriate measure depending on what type the message is
            case RecordMessage:
                // Create a new recorder and prepare it
                Log.Debug("DEBUG", "About to handle recording");
                return _handleRecordMessage(cTk);
            case EndRecordingMessage:
                // Attempt to end recording and return bytes
                return await _handleEndRecording(cTk);
            default:
                // This should never run tbh but to stop compilation errors
                throw new InvalidMessageException(message.GetType());
        }
    }
}