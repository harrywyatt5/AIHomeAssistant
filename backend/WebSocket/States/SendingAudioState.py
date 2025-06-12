from .BaseState import BaseState
from .WebSocketState import WebSocketState
from ..MessageTypes import MessageTypes

class SendingAudioState(BaseState):
    def __init__(self, audio_bytes):
        self.__ID__ = WebSocketState.SENDING_AUDIO
        self.audio_bytes = audio_bytes
        self.has_sent = False
        self.can_receive = [MessageTypes.CLIENT_ACK_RECEIVE_AUDIO, MessageTypes.CLIENT_FINISHED]
        

    async def manage_state(self, message):
        message_type, message = self.attempt_handle_message(message)

        if message_type == MessageTypes.CLIENT_ACK_RECEIVE_AUDIO and not self.has_sent:
            # Let's send the audio data
            self.has_sent = True
            return (self, self.audio_bytes)
        elif message_type == MessageTypes.CLIENT_FINISHED and self.has_sent:
            # Reset self
            # Hacky, I do apologise
            from .IdleState import IdleState
            return (IdleState(), None)



    def manange_button(self, button_state):
        # This state does not manage the button
        return None