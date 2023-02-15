import Structure_class as s
import re


def parser_computer(json: dict):
    name = json['Computer']['Name']
    ip_address = json['Computer']['IPAddress']
    manufacture = json['Computer']['Manufacturer']
    serial_number = json['Computer']['Serialnumber']
    monitors = list()
    for x in json['Computer']['Monitors']:
        monitor = s.Monitor(x['Manufacture'], x['Serialnumber'])
        monitors.append(monitor)
        print(monitor.serial_number.replace("\u0000", ""))

    serial_number = re.sub(r'[@{}]', '', serial_number)
    serial_number = serial_number.split("=")
    computer = s.Computer(name, ip_address, manufacture, serial_number[1])
    computer.add_monitors(monitors)
    print(computer.serial_number)

