import enum

class WebSocketState(enum.Enum):
    IDLE = 0
    RECORDING = 1
    AWAITING_RECORDING = 2
    SENDING_AUDIO = 3
    UNKNOWN = 100


