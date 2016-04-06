

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

	public static string candidateName;

	private static bool resetSceneFlag = false;
	private static bool playingBackgroundAudio = false;


	public static List<string> camerasToUpdate;

	public static float growStrength;
	public static float growRadius;
	public static float shrinkStrength;
	public static float shrinkRadius;

	#endregion


	public void Init()
	{
		// EDIT THIS
		// ALSO we have to set index in InitializeCamera.cs

		//candidateName = "targetCandidate";
		candidateName = "thermoCandidate";

		growStrength = 1.0f;
		growRadius = 0.02f;
		shrinkStrength = -1.0f; 
		shrinkRadius = 0.02f;


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

			for (int i = camerasToUpdate.Count - 1; i >= 0; i--)
			{
				// some code
				string c = camerasToUpdate[i];
				camerasToUpdate.RemoveAt(i);

				tryHitForCamera (c, shrinkStrength, shrinkRadius, false);
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
		// hit our camera
		tryHitForCamera(Camera.main.name, growStrength, growRadius, true);
	
	}

	void tryHitForCamera(string c, float s, float r, bool playsound) {
		GameObject cameraObject = GameObject.Find (c);
		GameObject candidate = GameObject.Find (candidateName);

		//handle their gaze
		if (cameraObject) {
			RaycastHit rh = GazeMeshModellerFunctions.GazeUpdate (cameraObject, candidate, s, r);
			if(rh.collider != null) {
				
				print ("gazehit: " + c + " hit something!");

				if (playsound) {
					print ("playing sound");
					GazeSoundFunctions.PlayRandomHitAtHit (rh);
				}
			}
		} else {
			print (c + " doesn't exist!!!!");
		}
			

	}

}
