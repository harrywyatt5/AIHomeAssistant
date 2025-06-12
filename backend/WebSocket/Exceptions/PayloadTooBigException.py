class PayloadTooBigException(Exception):
    def __init__(self, size):
        super().__init__(f'Request was over {size} bytes :()')