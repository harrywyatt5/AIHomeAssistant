from .MessageTypes import MessageTypes

class MessageCreator(object):
    @staticmethod
    def create_start_recording_message():
        return {
            "message_type": MessageTypes.CLIENT_START_RECORDING.value,
            "is_ack": False
        }
    
    
    @staticmethod
    def create_stop_recording_message():
        return {
            "message_type": MessageTypes.CLIENT_END_RECORDING.value,
            "is_ack": False
        }
    

    @staticmethod
    def create_prepare_send_audio_message(byte_count):
        # THis is for preparing the client
        return {
            "message_type": MessageTypes.CLIENT_START_RECEIVE_AUDIO.value,
            "bytes_to_receive": byte_count,
            "is_ack": False
        }
    

    @staticmethod
    def create_ready_to_receive(byte_count):
        # This is mostly sayijng to the client we are ready
        # Mostly because sockets are event based
        return {
            "message_type": MessageTypes.CLIENT_READY_TO_SEND.value,
            "bytes_to_receive": byte_count,
            "is_ack": False
        }