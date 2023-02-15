class Computer:
    name = None
    ip = None
    manufacture = None
    serial_number = None
    monitors = list()

    def __init__(self, name: str, ip: str, manufacture: str, serialnumber: str) -> None:
        self.name = name
        self.ip = ip
        self.manufacture = manufacture
        self.serial_number = serialnumber

    def add_monitors(self, monitors: list) -> None:
        self.monitors = monitors

    def __del__(self):
        print("Byl zničen objekt Computer!!")


class Monitor:
    manufacture = None
    serial_number = None

    def __init__(self, manufacture: str, serilanumber: str) -> None:
        self.manufacture = manufacture
        self.serial_number = serilanumber

    def __del__(self):
        print("Byl zničen objekt Monitor!!")
