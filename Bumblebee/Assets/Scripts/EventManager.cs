using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EventManager : MonoBehaviour {

	public SkinProcessor skinProcessor;
	public HeartProcessor heartProcessor;

	public Text statusText;
	public Text scoreText;
	public float scrollSpeed = -1.5f;
	public bool gameOver = false; 

	private int score = 0; 
	private Dictionary <string, UnityEvent> eventDictionary;
	private static EventManager eventManager;

	private float startTime;

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

	void Start (){
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
		float now = Time.time;
		float timePassed = now - startTime;

		isBaseline = ((timePassed > baselineStart) && (timePassed < baselineEnd)) ? true : false;
		isExcitement = ((timePassed >= excitementStart) && (timePassed < gameStart )) ? true : false;
		isGame = ((timePassed >= gameStart) && (timePassed < gameEnd)) ? true : false;

		if (isBaseline && baselineEventStart) {
			baselineEventStart = false;
			skinProcessor.ToggleBaseline ();
			statusText.text = "baseline aqusition";
		} else if (isExcitement && excitementEventStart) {
			excitementEventStart = false;
			skinProcessor.ToggleBaseline ();
			skinProcessor.ToggleExcitement();
			statusText.text = "excitement incoming";
			// play the excitement clip
		} else if (isGame && gameEventStart) {
			skinProcessor.ToggleExcitement ();
			skinProcessor.CalculateMinMax ();
			statusText.text = "this is a game";
			//set up game or graphs
		}else if (timePassed >= gameEnd){
			//show evaluation screen
			//cleanUp and restart the session
			//take care to log all important events
		}

	}

	void SetAnticipationDuration (){
		anticipationDuration = Random.Range (1.0f, 5.0f); 
	}


	public static EventManager instance
	{
		get
		{
			if (!eventManager)
			{
				eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

				if (!eventManager)
				{
					Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
				}
				else
				{
					eventManager.Init (); 
				}
			}

			return eventManager;
		}
	}

	void Init ()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	public static void StartListening (string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}

	public static void StopListening (string eventName, UnityAction listener)
	{
		if (eventManager == null) return;
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public static void TriggerEvent (string eventName)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke ();
		}
	}

	public void ScoreChange (int points){
		score += points;
		scoreText.text = "score: " + score;
	}
}