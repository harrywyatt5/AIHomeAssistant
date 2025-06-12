import asyncio
import grovepi

class ButtonState(object):
    def __init__(self):
        self.button_state = False
        self.lock = asyncio.Lock()
        grovepi.pinMode(4, "INPUT")


    async def check_button_state(self):
        async with self.lock:
            return self.button_state


    async def button_runtime(self):
        while True:
            # Check the button state from the pi, then update accordingly
            new_state = grovepi.digitalRead(4)

            async with self.lock:
                if (not bool(new_state)) != self.button_state:
                    self.button_state = (not bool(new_state))
                    print(f"Button is now {self.button_state}")

            await asyncio.sleep(0.1)