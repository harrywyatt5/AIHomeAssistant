from .BaseState import BaseState
from .WebSocketState import WebSocketState
from ..Exceptions.PayloadTooBigException import PayloadTooBigException
from ..Exceptions.PayloadTooSmallException import PayloadTooSmallException
from ..SpeechServices import SpeechServices
from .SendingAudioState import SendingAudioState
from ..MessageCreator import MessageCreator
import aiofiles
import os
import uuid
import tempfile
import asyncio

class ReceiveAudioState(BaseState):
    @staticmethod
    async def __save_file__(bytes):
        # Generate file and name
        file_path = ReceiveAudioState.__get_temp_filename__()
        ogg_file = file_path + ".ogg"
        flac_file = file_path + ".flac"

        async with aiofiles.open(ogg_file, mode='wb') as f:
            await f.write(bytes)
        
        # Convert it
        subprocess = await asyncio.create_subprocess_shell(
            f"ffmpeg -i {ogg_file} -c:a flac {flac_file}"
        )
        await subprocess.wait()

        return flac_file


    @staticmethod
    async def __load_file__(file):
        async with aiofiles.open(file, mode='rb') as f:
            return await f.read()


    @staticmethod
    def __get_temp_filename__():
        return os.path.join(tempfile.gettempdir() , str(uuid.uuid4()))


    def __init__(self, bytes_to_receive):
        self.__ID__ = WebSocketState.AWAITING_RECORDING
        self.bytes_to_receive = bytes_to_receive
        self.__bytes_buffer__ = b''
        self.can_receive = []  # This state does not receive any 'messages' (only binary)


    def manage_button(self, button_state):
        return None   # This state does not manage the button


    async def manage_state(self, message):
        # Put the bytes into the buffer
        self.__bytes_buffer__ += message

        if len(self.__bytes_buffer__) > self.bytes_to_receive:
            raise PayloadTooBigException(self.bytes_to_receive)
        elif len(self.__bytes_buffer__) == self.bytes_to_receive:
            # Save the audio clip and trigger detection code
            filename = await ReceiveAudioState.__save_file__(self.__bytes_buffer__)

            # Attempt to get text of recording
            service = SpeechServices.get_instance()
            speech_as_string = service.speech2text(filename)

            # Feed this text into ChatGPT
            ai_response = service.get_ai_response(speech_as_string)

            # Get speech synthesised
            file_loc = ReceiveAudioState.__get_temp_filename__()
            await service.text2speech(ai_response, file_loc)

            # Load the data
            response_data = await ReceiveAudioState.__load_file__(file_loc + ".flac")

            # Move to next state
            return (
                SendingAudioState(response_data),
                MessageCreator.create_prepare_send_audio_message(len(response_data))
            )
        else: 
            # Payload was too small! 
            raise PayloadTooSmallException(self.__bytes_buffer__, self.bytes_to_receive)






