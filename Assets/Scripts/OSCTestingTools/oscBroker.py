#!/usr/local/bin/python
import OSC
import re
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
    global multiclient
    print "== camera positionc allback"
    print path
    print tags
    print args
    print source
    print "yooo"
    print multiclient.getOSCTargets()

    oscmsg = OSC.OSCMessage('/test/')
    oscmsg.append("multiclient send")
    multiclient.send(oscmsg)

def reset_handler(path, tags, args, client_address):
    print "RESET HERE"

def default_handler(path, tags, args, client_address):
    global server, multiclient
    match = re.match("/SWG/camera/(\w*)/position", path)
    if(match):
        cameraName = match.groups()[0]

        print client_address
        print "ADDING " + cameraName + " TO callbacks"

        server.addMsgHandler( "/SWG/camera/" + cameraName + "/position", camera_position_callback )
        multiclient.setOSCTarget((client_address[0], oscPort))


##### RUN SERVER

multiclient = OSC.OSCMultiClient()

server = OSC.OSCServer( (oscHost, oscPort), client = multiclient, return_port = oscPort )
server.timeout = 0
server.run = True


server.addMsgHandler('default', default_handler)
server.addMsgHandler('/reset', reset_handler)

while server.run:
    server.handle_request()

server.close()


