#!/usr/local/bin/python
import OSC
import time
import argparse


## parse arguments
parser = argparse.ArgumentParser(description='Receive OSC.')
parser.add_argument('-host', nargs=1, required=True, help='host to receive OSC to')
parser.add_argument('-port', nargs=1, required=True, help='port number to receive OSC on')
args = parser.parse_args()

oscHost = args.host[0]
oscPort = int(args.port[0])


##### HANDLING

def quit_callback(path, tags, args, source):
    global server
    server.run = False

def camera_position_callback(path, tags, args, source):
    print "camera positionc allback"
    print path
    print tags
    print args
    print source

def camera_init_callback(path, tags, args, source):
    global server

    cameraname = args[0]
    server.addMsgHandler( "/SWG/camera/" + cameraname + "/position", camera_position_callback )
    
    print "ADDING " + cameraname + " TO callbacks"



##### RUN SERVER

multiclient = OSC.OSCMultiClient()

server = OSC.OSCServer( (oscHost, oscPort), client = multiclient, return_port = oscPort )
server.timeout = 0
server.run = True


server.addMsgHandler( "/SWG/camera/addme", camera_init_callback )

while server.run:
    server.handle_request()

server.close()

