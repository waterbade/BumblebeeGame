using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OldEventManager : MonoBehaviour {

	/*public static EventManager instance; 
	private Text statusText;

	private Dictionary <string, UnityEvent> eventDictionary;
	private static EventManager eventManager;

	private float startTime;
	private bool toGame;
	private bool setup = false;
	public bool playVideo = false;

	//times in seconds
	private float calmingDuration = 2.00f; 
	private float baselineDuration = 2.00f;//30.00f;
	private float anticipationDuration = 3.00f; 
	private float excitementDuration = 10.00f;//60.00f;
	private float gamesessionDuration = 180.00f;

	// time when something starts
	private float baselineStart;
	private float baselineEnd;
	private float excitementStart;
	private float gameStart;
	private float gameEnd;

	private bool isBaseline = false;
	private bool isExcitement = false;
	private bool isGame = false;

	private bool baselineEventStart = true;
	private bool excitementEventStart = true;
	private bool gameEventStart = true;

	public float GetGameSessionDuration(){
		return gamesessionDuration;
	}

	void Awake()
	{
		//SingletonPattern for EventManager
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this.gameObject);
	}

	void Start (){
		statusText = GameObject.FindGameObjectWithTag ("statusText").GetComponent<Text>();
		startTime = Time.time;
		SetAnticipationDuration ();

		baselineStart = calmingDuration;
		baselineEnd = calmingDuration + baselineDuration;
		excitementStart = calmingDuration + baselineDuration + anticipationDuration;
		gameStart = calmingDuration + baselineDuration + anticipationDuration + excitementDuration;
		gameEnd = calmingDuration + baselineDuration + anticipationDuration + excitementDuration + gamesessionDuration;
		statusText.text = "startUp";
	}

	void Update() {
		if (setup) {
			if (SkinProcessor.instance != null) {
				float now = Time.time;
				float timePassed = now - startTime;
				CheckTimeForEvents (timePassed);
			}
		}
	}

	public void switchToSetupForGame(){
		toGame = true;
		setup = true;
		SceneManager.LoadScene ("setup");

	}

	public void switchToSetupForGraphs(){
		toGame = false;
		setup = true;
		SceneManager.LoadScene ("setup");
	}


	void CheckTimeForEvents(float timePassed){

		isBaseline = ((timePassed > baselineStart) && (timePassed < baselineEnd)) ? true : false;
		isExcitement = ((timePassed >= excitementStart) && (timePassed < gameStart )) ? true : false;
		isGame = ((timePassed >= gameStart) && (timePassed < gameEnd)) ? true : false;

		// Getting the baseline
		if (isBaseline && baselineEventStart) {
			baselineEventStart = false;
		}

		// Arousing the User
		else if (isExcitement && excitementEventStart) {
			excitementEventStart = false;

			// play the excitement clip
		}

		//startng the biofeedback- session
		else if (isGame && gameEventStart) {
			statusText.text = "		this is a game";
			//set up game or graphs
		}

		//ending a trial
		else if (timePassed >= gameEnd){
			//show evaluation screen
			//cleanUp and restart the session
			//take care to log all important events
		}
	}


	void SetAnticipationDuration (){
		anticipationDuration = Random.Range (1.0f, 5.0f); 
	}

	private void SwitchToGame(){
		SceneManager.LoadScene ("game");
	}

	private void SwitchToGraphs(){
		SceneManager.LoadScene ("graphs");
	}

	private void SwitchToStart(){
		SceneManager.LoadScene ("start");
		setup = false;
	}*/

}