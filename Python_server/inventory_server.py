import socket
import sys
import select
import json

HOST = '127.0.0.1' 
SOCKET_LIST = []
KLIENT_LIST = []
RECV_BUFFER = 4096 
PORT = 11111


def chat_server():
    
    Name_bool = False
    name = None
    
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind((HOST, PORT))
    server_socket.listen(100)
    
    SOCKET_LIST.append(server_socket)
    
    print("Server start listen on port: " + str(PORT))
    
    while True:
        ready_to_read,ready_to_write,in_error = select.select(SOCKET_LIST,[],[],0)
        
        for sock in ready_to_read:
            # a new connection request recieved
            if sock == server_socket: 
                sockfd, addr = server_socket.accept()
                SOCKET_LIST.append(sockfd)
                print("Client (%s, %s) connected" % addr)
                sockfd.send("LKQ2023I".encode())
                client = dict()
                client['fd'] = sockfd
                client['name'] = None
                KLIENT_LIST.append(client)
            else:
                data = sock.recv(RECV_BUFFER)
                if data:
                    for x in KLIENT_LIST:
                        if(sock == x['fd']):
                            if(x['name'] == None):
                                x['name'] = str(data.decode())
                            else:
                                data = json.loads(data)
                                with open(str(x['name']) + ".json", "w") as wf:
                                    json.dump(data, wf, indent=3)
                              
                else:
                    if sock in SOCKET_LIST:
                        SOCKET_LIST.remove(sock)
                        print("Client was discconnect")
                        for x in KLIENT_LIST:
                            if(sock == x['fd']):
                                KLIENT_LIST.remove(x)
                    
                    
                        
    
    
if __name__ == '__main__':
    sys.exit(chat_server())
        