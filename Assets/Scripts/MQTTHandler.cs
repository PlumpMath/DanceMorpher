using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

public class MQTTHandler : MonoBehaviour {

	#region Singleton Constructors
	static MQTTHandler()
	{
	}

	MQTTHandler()
	{
	}

	public static MQTTHandler Instance 
	{
		get 
		{
			if (_instance == null) 
			{
				_instance = new GameObject ("MQTTHandler").AddComponent<MQTTHandler>();
			}

			return _instance;
		}
	}
	#endregion


	#region Member Variables
	private static MQTTHandler _instance = null;

	private const int _loglength = 25;


	public static GameObject theirCameraObject = null;
	public static float myStrength = 0.0f; 
	public static float myRadius = 0.0f;
	public static float theirStrength = 0.0f; 
	public static float theirRadius = 0.0f;

	private static string setCamerasString = ""; 


	public static string serverListenHost = "vps.provolot.com";
	public static string localServerListenHost = "192.168.1.200";

	public static int serverListenPort = 1883;

	public static string myCameraName;


	#endregion

	/**********/
	// receives /DanceMorpher/cameras/positions
	// sends /DanceMorpher/camera/$CAMERANAME/position


	private MqttClient client;
	// Use this for initialization
	void Start () {

		// create client instance 

		//Initialize OSC clients (transmitters)

		client = new MqttClient(serverListenHost, serverListenPort  , false , null ); 

		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { "/DanceMorpher/cameras/positions" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
		client.Subscribe(new string[] { "/DanceMorpher/resetscenedontusethis" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
		client.Subscribe(new string[] { "/DanceMorpher/reloadmesh" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

	
		InvokeRepeating("sendCameraMessage", 0, 0.25F);


	}
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 

		Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
		Debug.Log(e.Topic);

		if (e.Topic == "/DanceMorpher/cameras/positions") {
			string msg = System.Text.Encoding.UTF8.GetString(e.Message);
			print (msg);

			setCamerasString = msg;
			// this stuff will be handled by 'Update()' because Unity wants it so
		}



		if (e.Topic == "/DanceMorpher/resetscenedontusethis") {
			Debug.Log ("Reset Scene!!!");
			HitHandler.resetSceneDontUseThis();
		}

		if (e.Topic == "/DanceMorpher/reloadmesh") {
			Debug.Log ("Reload Mesh");
			HitHandler.reloadMesh();
		}
	} 

	void Update() {

		// this is a workaround because Unity wants 'Find' to be part of the main thread
		if (setCamerasString != "") {

			// set cameraString to be empty first, because if setCamera fails, this will loop infinitely)
			var cs = setCamerasString;
			setCamerasString = "";

			setCameras (cs);
		}

		if (myCameraName == null) {
			myCameraName = Camera.main.name; 
		}

	}


	string getTimestamp() {
		long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
		ticks /= 10000000; //Convert windows ticks to seconds
		var timestamp = ticks.ToString();
		return timestamp;
	}



	void setCameras(string camerasString) {
		string[] cameras = camerasString.Split (';');

		foreach (string c in cameras) {

			string[] pos = c.Split ('/');

			if (pos [0] != myCameraName) {

				print (pos [0]);
				print (pos [1]);
				print (pos [2]);

				setCameraPosition (pos [0], getVector3 (pos [1]), getVector3 (pos [2]));

				// ask HitHAndler to look for hits for these cameras
				HitHandler.lookForHit (pos [0]);


			}
		}

	}
		

	void sendCameraMessage() {

		//Debug.Log("sending...");

		if (myCameraName != null) {

			string cameraPosition = Camera.main.gameObject.transform.position.ToString ("F5");
			string cameraEuler = Camera.main.gameObject.transform.eulerAngles.ToString ("F5");

			string message = cameraPosition + "/" + cameraEuler + "/" + getTimestamp ();

			client.Publish ("/DanceMorpher/camera/" + myCameraName + "/position", System.Text.Encoding.UTF8.GetBytes (message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		
		}

		//Debug.Log("sent");
	
	}

	public Vector3 getVector3(string rString){
		string[] temp = rString.Substring(1,rString.Length-2).Split(',');
		float x = float.Parse(temp[0]);
		float y = float.Parse(temp[1]);
		float z = float.Parse(temp[2]);
		Vector3 rValue = new Vector3(x,y,z);
		return rValue;
	}


	void setCameraPosition(string cameraName, Vector3 position, Vector3 eulerAngles) {
		GameObject cameraObjToSet = GameObject.Find (cameraName + "_Model");


		cameraObjToSet.gameObject.transform.position = position;
		cameraObjToSet.gameObject.transform.eulerAngles = eulerAngles;

		// we store the Cameraindex in a static Var so that handleGazesHit() can handle it
		//print("camera>>>");
		//print(cameraID);
		//theirCameraObject = cameraToSet;
	}




}
