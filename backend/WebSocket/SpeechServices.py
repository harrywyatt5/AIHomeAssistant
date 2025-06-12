import openAIconnection
import speechToText
import t2s

class SpeechServices(object):
    __singleton__ = None

    @staticmethod
    def get_instance():
        if not SpeechServices.__singleton__:
            # Initialise ai services
            ai = openAIconnection.initialise()

            SpeechServices.__singleton__ = SpeechServices(ai, speechToText.convert, t2s.text2speech)

        return SpeechServices.__singleton__

    
    def __init__(self, ai_client, speech_to_text_function, text_to_speech):
        self.ai_client = ai_client
        self.speech2text = speech_to_text_function
        self.text2speech = text_to_speech


    def get_ai_response(self, prompt):
        return openAIconnection.query(self.ai_client, prompt)