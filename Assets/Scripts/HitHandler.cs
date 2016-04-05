

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


	#endregion


	public void Init()
	{
		// EDIT THIS
		// ALSO we have to set index in InitializeCamera.cs

		//candidateName = "targetCandidate";
		candidateName = "thermoCandidate";


		// THE BELOW STUFF JUST MAKES OUR LIVES EASIER. DON'T EDIT.
		float growStrength = 40.2f; 
		float growRadius = 10.2f;
		float shrinkStrength = -7.2f;
		float shrinkRadius = 10.2f;


		myRadius = growRadius;
		myStrength = growStrength;

	}

	void Start() {
		HitHandler.Instance.Init();

		InvokeRepeating ("handleGazesHit", 1, 1.25F);
	
		GazeSoundFunctions.PlayBackgroundAtObject( Camera.main.gameObject);

	}

	void Update() {
	}

	public static void resetMesh() {
		Debug.Log ("========== Resetting scene");

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void handleGazesHit() {

		GameObject candidate = GameObject.Find (candidateName);


		print("handleGazesHit()");

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
