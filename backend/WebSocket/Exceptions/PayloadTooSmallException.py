class PayloadTooSmallException(Exception):
    def __init__(self, got, expected):
        super().__init__(f'Unexpected payload size. Expected {expected} bytes and got {got}')