using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	public static GameController instance;      //A reference to our game control script so we can access it statically.
	public Text scoreText;						//A reference to the UI text component that displays the player's score.
	public Image honey; 
	private float score = 0f;                      //The player's score.
	private float maxScore = 100f;
	public float scrollSpeed = -1.5f;


	void Awake()
	{
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
	}

	void Start(){
		honey.fillAmount = 0.0f;
	}

	public void ScoreChange (int points){
		score += points;
		int currentScore = (int)score;
		scoreText.text = currentScore.ToString();
		float fill = score / maxScore;
		honey.fillAmount = fill;
		EventManager.instance.score = (int) score;

	}
}