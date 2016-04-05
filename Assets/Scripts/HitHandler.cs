

using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;


public class HitHandler : MonoBehaviour
{
	#region Singleton Constructors
	static HitHandler()
	{
	}

	HitHandler()
	{
	}
	
	public static HitHandler Instance 
	{
	    get 
		{
	        if (_instance == null) 
			{
				_instance = new GameObject ("HitHandler").AddComponent<HitHandler>();
	        }
	       
	        return _instance;
	    }
	}
	#endregion
	
	#region Member Variables
	private static HitHandler _instance = null;

	private const int _loglength = 25;


	public static GameObject theirCameraObject = null;
	public static float myStrength = 0.0f; 
	public static float myRadius = 0.0f;
	public static float theirStrength = 0.0f; 
	public static float theirRadius = 0.0f;
	public static string candidateName;

	private static bool resetSceneFlag = false;
	private static bool playingBackgroundAudio = false;


	public static List<string> camerasToUpdate;


	#endregion


	public void Init()
	{
		// EDIT THIS
		// ALSO we have to set index in InitializeCamera.cs

		//candidateName = "targetCandidate";
		candidateName = "targetCandidate2";


		// THE BELOW STUFF JUST MAKES OUR LIVES EASIER. DON'T EDIT.
		float growStrength = 40.2f; 
		float growRadius = 10.2f;
		float shrinkStrength = -7.2f;
		float shrinkRadius = 10.2f;


		myRadius = growRadius;
		myStrength = growStrength;

		camerasToUpdate = new List<string>();

	}

	void Start() {
		HitHandler.Instance.Init();

		InvokeRepeating ("handleGazesHit", 1, 0.5F);

	}

	void Update() {

		if (resetSceneFlag) {
			resetSceneFlag = false;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		if (!playingBackgroundAudio && Camera.main.gameObject != null) {
			GazeSoundFunctions.PlayBackgroundAtObject (Camera.main.gameObject);
			print ("playing");
			playingBackgroundAudio = true;
		}

		if (camerasToUpdate.Count > 0) {
			foreach (string c in camerasToUpdate) {
				
				print (c);
				camerasToUpdate.RemoveAt (0);
			}

		}

	}

	public static void resetSceneDontUseThis() {
		Debug.Log ("========== Resetting scene");
		// don't use this, because this will offset the orientation of cameras from the moving rack.

		resetSceneFlag = true;
	}

	public static void reloadMesh() {
		Debug.Log ("========== reloading mesh");

		// RELOAD MESH STUFF HERE

	}

	public static void lookForHit(string cameraName) {
		// put this in a queue and look for hit through update()

		camerasToUpdate.Add (cameraName);
	}


	void handleGazesHit() {

		GameObject candidate = GameObject.Find (candidateName);

		//handle our gaze
		RaycastHit ourRaycastHit = GazeMeshModellerFunctions.GazeUpdate (Camera.main.gameObject, candidate, myStrength, myRadius);

		if (ourRaycastHit.collider != null) {
			try {
				print ("gazehit: WE hit something!");
				print (ourRaycastHit.ToString ());

				print ("playing sound");
				GazeSoundFunctions.PlayRandomHitAtHit(ourRaycastHit);

				print (GazeSoundFunctions.RandomHitClip());


			} catch {
			}
		}

		//handle their gaze
		if (theirCameraObject) {

			RaycastHit theirRaycastHit = GazeMeshModellerFunctions.GazeUpdate (theirCameraObject, candidate, theirStrength, theirRadius);

			if(theirRaycastHit.collider != null) {


				print ("gazehit: THEY hit something!");
			}
		} else {
			print ("theirCameraObject doesn't exist!!!!");
		}



	}
}
