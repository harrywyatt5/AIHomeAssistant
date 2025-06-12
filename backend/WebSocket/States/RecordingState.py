from .BaseState import BaseState
from .WebSocketState import WebSocketState
from .ReceiveAudioState import ReceiveAudioState
from ..MessageTypes import MessageTypes
from ..Exceptions.InvalidMessageReceived import InvalidMessageReceived
from ..MessageCreator import MessageCreator

class RecordingState(BaseState):
    def __init__(self):
        self.__ID__ = WebSocketState.RECORDING
        self.dispatched_cancel = False  # Whether we have told the client to stop recording
        self.can_receive = [MessageTypes.CLIENT_ACK_END_RECORDING]


    def manage_button(self, current_state):
        # If the button has been turned off in this state...
        # Then we need to tell the client to stop recording
        if not current_state and not self.dispatched_cancel:
            self.dispatched_cancel = True
            return (self, MessageCreator.create_stop_recording_message())

        return None


    async def manage_state(self, message):
        # Only accept messages if we have already told the client to cancel their recording
        if self.dispatched_cancel:
            message_type, deserialized = self.attempt_handle_message(message)

            # If we have a valid message type it's because the client has confirmed and will send audio data
            if message_type:
                # Check it was an ACK message
                if not deserialized.get("is_ack"):
                    raise InvalidMessageReceived("Must be an ACK")
                
                # Check how many bytes we need (if we even received it)
                bytes_to_receive = deserialized.get("bytes_to_send")
                if not bytes_to_receive:
                    raise InvalidMessageReceived("bytes_to_send must be included in message")


                return (ReceiveAudioState(bytes_to_receive), MessageCreator.create_ready_to_receive(bytes_to_receive))


    def __eq__(self, value):
        if type(value) == str:
            return value == self.__ID__.name
        
        return type(value) == type(self)