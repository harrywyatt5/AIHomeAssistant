import asyncio
import websockets.server
from .States.IdleState import IdleState
from .States.RecordingState import RecordingState
import json


class WebSocketRuntime(object):
    def __init__(self):
        self.state = None
        self.ai = None
        self.button_state_manager = None


    async def start_server(self, host_location, port, button, ai):
        self.button_state_manager = button
        self.ai = ai
        self.state = IdleState()

        async with websockets.server.serve(self._handler, host_location, port):
            await asyncio.Future()


    async def __send_message__(self, websocket, message):
        # attempts to see if the message is a dict or binary and sends accordingly
        inner_message = message
        if type(inner_message) == dict:
            inner_message = json.dumps(inner_message)

        await websocket.send(inner_message)


    async def _handler(self, websocket):
        # Intialise state machine
        while True:
            try:
                # Check button state
                action = self.state.manage_button(await self.button_state_manager.check_button_state())
                if action != None:
                    self.state = action[0]
                    print("Sending stuff to client")
                    await self.__send_message__(websocket, action[1])


                # Await for our message for only 200 miliseconds and then restart the loop
                message = None
                try:
                    message = await asyncio.wait_for(websocket.recv(), timeout=0.2)
                except Exception as e:
                    if type(e) == websockets.ConnectionClosedOK:
                        raise  # Rethrow
                    
                    # Otherwise, continue
                    continue
                

                # Manage request, returns a tuple describing message and new state, or None
                response_context = await self.state.manage_state(message)
                print(response_context)
                if response_context:
                    new_state, response = response_context
                    print(f"Next state: {new_state}")
                    self.state = new_state

                    # If we've got a response to give...
                    if response:
                        await self.__send_message__(websocket, response)
            except websockets.ConnectionClosedOK as e:
                break
