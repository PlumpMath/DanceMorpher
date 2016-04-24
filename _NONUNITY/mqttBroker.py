import mosquitto, os, urlparse
import re

cameraPoses = {}

# Define event callbacks
def on_connect(mosq, obj, rc):
    print("rc: " + str(rc))

def on_message(mosq, obj, msg):
    match = re.match("/DanceMorpher/camera/(.*)/position", msg.topic)
    if(match):
        cameraName = match.groups()[0]
        cameraPoses[cameraName] = cameraName + "/" + msg.payload

    sendCameraPoses()

def on_publish(mosq, obj, mid):
    print("mid: " + str(mid))

def on_subscribe(mosq, obj, mid, granted_qos):
    print("Subscribed: " + str(mid) + " " + str(granted_qos))

def on_log(mosq, obj, level, string):
    print(string)

def sendCameraPoses():
    cps = ";".join(cameraPoses.values())
    mqttc.publish("/DanceMorpher/cameras/positions", cps)

mqttc = mosquitto.Mosquitto()
# Assign event callbacks
mqttc.on_message = on_message
mqttc.on_connect = on_connect
mqttc.on_publish = on_publish
mqttc.on_subscribe = on_subscribe

# Uncomment to enable debug messages
#mqttc.on_log = on_log

# Parse CLOUDMQTT_URL (or fallback to localhost)
url_str = os.environ.get('CLOUDMQTT_URL', 'mqtt://vps.provolot.com:1883')
url = urlparse.urlparse(url_str)

# Connect
mqttc.username_pw_set(url.username, url.password)
mqttc.connect(url.hostname, url.port)

# Start subscribe, with QoS level 0
mqttc.subscribe("/DanceMorpher/camera/Main Camera 0/position", 0)
#mqttc.subscribe("/DanceMorpher/camera/Main Camera 1/position", 0)
#mqttc.subscribe("/DanceMorpher/camera/Main Camera 2/position", 0)
#mqttc.subscribe("/DanceMorpher/camera/Main Camera 3/position", 0)
mqttc.subscribe("/DanceMorpher/camera/Main Camera 4/position", 0)
#mqttc.subscribe("/DanceMorpher/camera/Main Camera 5/position", 0)

# Publish a message
#mqttc.publish("hello/world", "my message")

# Continue the network loop, exit when an error occurs
rc = 0
while rc == 0:
    rc = mqttc.loop()
print("rc: " + str(rc))

