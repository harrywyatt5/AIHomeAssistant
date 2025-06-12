using System.Text.Json.Serialization;

namespace IOTProjectApp.WebSocket.Message;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "message_type")]
[JsonDerivedType(typeof(BaseControlMessage), (int)ControlMessageType.BaseMessage)]
[JsonDerivedType(typeof(RecordMessage), (int)ControlMessageType.StartRecording)]
[JsonDerivedType(typeof(EndRecordingMessage), (int)ControlMessageType.EndRecording)]
[JsonDerivedType(typeof(ServerReadyToReceive), (int)ControlMessageType.ServerReadyToReceive)]
[JsonDerivedType(typeof(EndRecordingAck), (int)ControlMessageType.EndRecordingAck)]
[JsonDerivedType(typeof(ReceiveAudioMessage), (int)ControlMessageType.ReceiveAudio)]
[JsonDerivedType(typeof(ReceiveAudioMessageAck), (int)ControlMessageType.ReceiveAudioAck)]
[JsonDerivedType(typeof(TransactionCompleteMessage), (int)ControlMessageType.TransactionComplete)]
public class BaseControlMessage
{
    public bool IsAck { get; set; }
}