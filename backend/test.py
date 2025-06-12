import openAIconnection
import speechToText


client = openAIconnection.initialise()
message = speechToText.convert("/Audio/peter-the-horse-is-here.flac")
response = openAIconnection.query(client, message)
print(response)