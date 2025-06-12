import pyttsx3
import asyncio
import os

async def text2speech(text, filename):
    engine = pyttsx3.init()
    final_name = filename + ".flac"
    engine.save_to_file(text, final_name)
    engine.runAndWait()

    # For some reason, sometimes the file isn't made at the time of completion. We will wait
    # A bit hacky again, sorry
    for i in range(100):
        # If in final iteration, print a warning
        if i == 99:
            print("WARN: Been waiting for speech file for 5 seconds. Something's wrong!")

        all_in_dir = os.listdir("/tmp/")

        if final_name in all_in_dir:
            print("Hooray!")
            break
        
        await asyncio.sleep(0.05)

