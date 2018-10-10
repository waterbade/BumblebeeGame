using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	public Text BaselineAvg;
	public Text FeedbackAvg;
	public Text score;
	public Button restartButton;

	void Update(){
		if (EventManager.instance.end) {
			EventManager.instance.end = false;
			SetScore ();

			if (TrialManager.instance.currentStrategy.Count > 0)
				restartButton.onClick.AddListener (EventManager.instance.SwitchToStart);
		}
	}
	// Use this for initialization
	private void SetScore(){
		BaselineAvg.text = EventManager.instance.baselineAverage.ToString();
		FeedbackAvg.text = EventManager.instance.feedbackAverage.ToString ();
		score.text = EventManager.instance.score.ToString ();

		TrialManager.instance.SaveScores (BaselineAvg.text, FeedbackAvg.text, score.text);
	}
		
}
