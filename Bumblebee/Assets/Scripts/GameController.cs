using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	public static GameController instance;      //A reference to our game control script so we can access it statically.
	public Text scoreText;						//A reference to the UI text component that displays the player's score.
	public Image honey; 
	private int score = 0;                      //The player's score.
	private int maxScore = 40;
	private int honeyChange;
	public float scrollSpeed = -1.5f;


	void Awake()
	{
		//If we don't currently have a game control...
		if (instance == null)
			//...set this one to be it...
			instance = this;
		//...otherwise...
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}

	void Start(){
		honey.fillAmount = 0.0f;
		honeyChange = 1/maxScore;

	}
	void Update()
	{honey.fillAmount = score / maxScore;
	}

	public void ScoreChange (int points){
		score += points;
		scoreText.text = score.ToString();
		honey.fillAmount = score / maxScore;
	}
}