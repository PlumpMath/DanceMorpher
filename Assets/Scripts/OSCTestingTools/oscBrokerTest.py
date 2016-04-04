#!/usr/local/bin/python


import OSC
import time
import argparse


## parse arguments

parser = argparse.ArgumentParser(description='Send OSC.')
parser.add_argument('-host', nargs=1, required=True, help='host to send OSC to')
parser.add_argument('-port', nargs=1, required=True, help='port number to send OSC on')
args = parser.parse_args()

oscHost = args.host[0]
oscPort = int(args.port[0])

cameraName = "CAMERA9"

## connect to OSC

c = OSC.OSCClient()
c.connect((oscHost, oscPort))  # connect to a Unity camera (OSC Server)

oscAddress = '/SWG/camera/addme'
oscMessage = cameraName

oscmsg = OSC.OSCMessage(oscAddress)
oscmsg.append(oscMessage)
c.send(oscmsg)

counter = 0

while(1):

    oscMessage = '(-23.16295, 1.55000, ' + str((counter/100.0) - 5.0) + ')'
    oscMessage2 = '(' + str((counter) * 10 % 360) + ', 261.26000, ' + str((counter) * 2 % 180) + ')'
    oscAddress = "/SWG/camera/" + cameraName + "/position"

    print counter , "sending", oscMessage, " to Unity to ", oscAddress
    oscmsg = OSC.OSCMessage(oscAddress)
    oscmsg.append(oscMessage)
    oscmsg.append(oscMessage2)
    c.send(oscmsg)

    counter = counter + 1
    time.sleep(0.5)

