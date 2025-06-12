from .WebSocketState import WebSocketState
from .BaseState import BaseState
from ..MessageTypes import MessageTypes
from .RecordingState import RecordingState
from ..MessageCreator import MessageCreator


class IdleState(BaseState):
    def __init__(self):
        self.__ID__ = WebSocketState.IDLE
        self.dispatched_record = False # Whether the request to record has been dispatched
        self.can_receive = [MessageTypes.CLIENT_START_RECORDING]


    def manage_button(self, current_state):
        # If the button is on
        if current_state and not self.dispatched_record:
            self.dispatched_record = True
            return (self, MessageCreator.create_start_recording_message())
                
        # Otherwise, no action needs to occur
        return None


    async def manage_state(self, message):
        # Only do something if we've dispatched a record request (waiting for it to be acknowledged before moving state)
        if self.dispatched_record:
            message_type, message = self.attempt_handle_message(message)

            if message_type:
                # If this is a valid message and we can handle it
                return (RecordingState(), None)


    def __eq__(self, value):
        if type(value) == str:
            return value == self.__ID__.name
        
        return type(value) == type(self)