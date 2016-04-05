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
	public static string candidateName;
	public static string myName;

	private static string setCamerasString = ""; 


	#endregion

	private MqttClient client;
	// Use this for initialization
	void Start () {



		// create client instance 

		//Initialize OSC clients (transmitters)

		var serverListenHost = "vps.provolot.com";
		var serverListenPort = 1883;

		myName = "computer"; 

		candidateName = "thermoCandidate";

		client = new MqttClient(serverListenHost, serverListenPort , false , null ); 
		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { "/DanceMorpher/cameras/positions" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
		client.Subscribe(new string[] { "/DanceMorpher/reset" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 


		InvokeRepeating("sendCameraMessage", 0, 2F);


	}
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 

		Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
		print (e.Topic);
		//print (System.Text.Encoding.UTF8.GetString (e.Topic));

		if (e.Topic == "/DanceMorpher/cameras/positions") {

			string msg = System.Text.Encoding.UTF8.GetString(e.Message);
			print (msg);

			setCamerasString = msg;
			// this stuff will be handled by 'Update()' because Unity wants it so
		}

		if (e.Topic == "/DanceMorpher/reset") {


			HitHandler.resetMesh();

		}
	} 

	void Update() {

		// this is a workaround because Unity wants 'Find' to be part of the main thread
		if (setCamerasString != "") {
			var cs = setCamerasString;
			setCamerasString = "";

			setCameras (cs);
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

			print (pos [0]);
			print (pos [1]);
			print (pos [2]);

			setCameraPosition (pos [0], getVector3 (pos [1]), getVector3 (pos [2]));
		}

	}
		

	void sendCameraMessage() {

		//Debug.Log("sending...");

		string cameraPosition = Camera.main.gameObject.transform.position.ToString("F5");
		string cameraEuler = Camera.main.gameObject.transform.eulerAngles.ToString("F5");

		string message = cameraPosition + "/" + cameraEuler + "/" + getTimestamp();

		client.Publish("/DanceMorpher/camera/" + myName + "/position", System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

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
		GameObject cameraToSet = GameObject.Find (cameraName);


		cameraToSet.gameObject.transform.position = position;
		cameraToSet.gameObject.transform.eulerAngles = eulerAngles;

		// we store the Cameraindex in a static Var so that handleGazesHit() can handle it
		//print("camera>>>");
		//print(cameraID);
		//theirCameraObject = cameraToSet;
	}

	void receiveCameraMessages() {
		/*
		OSCHandler.Instance.UpdateLogs();

		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;

		try {

			string oscAddress = servers["thisListener"].server.LastReceivedPacket.Address;
			string oscMessage = servers["thisListener"].server.LastReceivedPacket.Data[0].ToString();

			if(oscAddress == "/reset") {
				// if we receive a reset message, then reload level
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			} else {

				print(">>>>");

				print (oscMessage);
				print (oscAddress);


				Regex regex = new Regex(@"/camera/(?<cameraID>\w+?)/position");
				var match = regex.Match(oscAddress);
				var cameraID = match.Groups["cameraID"].Value;


				GameObject cameraToSet = GameObject.Find ("CAMERA" + cameraID);
				string[] positionorientation = oscMessage.Split('/');

				print(getVector3(positionorientation[0]));
				print(positionorientation[1]);

				cameraToSet.gameObject.transform.position = getVector3(positionorientation[0]);
				cameraToSet.gameObject.transform.eulerAngles = getVector3(positionorientation[1]);


				// we store the Cameraindex in a static Var so that handleGazesHit() can handle it
				print("camera>>>");
				print(cameraID);
				theirCameraObject = cameraToSet;
			}



		} catch(Exception e) {
			print ("Error happened: " + e);
		}

		*/
	}



}
