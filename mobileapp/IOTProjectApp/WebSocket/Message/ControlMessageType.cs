namespace IOTProjectApp.WebSocket.Message;

public enum ControlMessageType
{
    // Some messages use the same class for their ACK, so will not be mentioned twice
    BaseMessage,
    StartRecording,  // Ack is the same
    EndRecording,
    EndRecordingAck,
    ServerReadyToReceive,
    ReceiveAudio,
    ReceiveAudioAck,
    TransactionComplete
}