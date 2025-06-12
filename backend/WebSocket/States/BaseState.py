from .WebSocketState import WebSocketState
from ..MessageTypes import MessageTypes
import json
import abc

class BaseState(object):
    def __init__(self):
        self.__ID__ = WebSocketState.UNKNOWN
        self.can_receive = []
    

    def attempt_handle_message(self, message):
        # tries to deserialize and sees if we can handle the message
        try:
            deserialized_message = self.deserialize_message(message)
            print(f"Deserialized message is: {deserialized_message}")

            message_type = self.can_receive_message(deserialized_message.get("message_type"))
            if message_type:
                return (message_type, deserialized_message)
            else:
                return (None, deserialized_message)
        except json.JSONDecodeError:
            print("Input was not JSON!!!")
            return (None, None)
        

    def can_receive_message(self, message_id):
        # Attempt to get ID as message
        try:
            m_type = MessageTypes(message_id)

            if m_type in self.can_receive:
                return m_type
        except ValueError:
            # We don't want to handle this
            pass

        return None


    @abc.abstractmethod
    async def manage_state(self, message):
        pass


    @abc.abstractmethod
    def manage_button(self, current_state):
        pass


    def deserialize_message(self, message):
        return json.loads(message)
    

    def __eq__(self, value):
        if type(value) == str:
            return value == self.__ID__.name
        
        return type(value) == type(self)