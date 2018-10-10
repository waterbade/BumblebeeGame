using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public FlowerCreator flowerCreator;
	private Vector2[] flowerZonePositions;
	private BitalinoReader bitalinoReader;
	//movement variables
	private float moveRate = 2.0f;
	private Rigidbody2D rb;
	private float upperBoundScreen;
	private float lowerBoundScreen;
	private float halfBee; 
	//zoning
	private Vector2double [] SkinValueZones = new Vector2double[6];
	private double baselineAvg = 0f;
	private double standardDev = 0f;
	private int divider = 10;

	//control
	public GameObject[] zoneSprites;
	public bool standardDeviationAsZone;

	void Start () {
		
		rb = GetComponent<Rigidbody2D> ();
		halfBee = this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
		upperBoundScreen = Camera.main.orthographicSize - halfBee;
		lowerBoundScreen = -Camera.main.orthographicSize + halfBee;

		flowerZonePositions = flowerCreator.GetZonePositons ();

		bitalinoReader = BitalinoObject.instance.GetComponentInChildren<BitalinoReader> ();
		//SetupSkinZones ();
		//beeRenderer = this.GetComponent<SpriteRenderer> ();
		//beeAnimator = this.GetComponent<Animator> ();
	}
		
	void Update () {
		MoveCharacter ();
	}

	private void MoveCharacter(){
		if (bitalinoReader.asStart) {
			BitalinoMovement ();
		}
		else{
			KeyboardMovement ();
		}
	}

	//Move the Bee with the Keyboard
	private void KeyboardMovement(){
		float y = 0.0f;
		float move = Input.GetAxis ("Vertical");

		if (move < 0)
			y = -moveRate;
		else if (move > 0)
			y = moveRate;
	
		rb.velocity = new Vector2(0, y);
		CheckBounds (lowerBoundScreen, upperBoundScreen);

	}

	private void BitalinoMovement (){
		float y = 0f; 

		Vector2 playerZone = ZoneOfPlayer ();
		Vector2 valueZone = SkinProcessor.instance.GetSkinValueZone ();
		//Vector2 valueZone = ZoneOfSkinValue (SkinProcessor.instance.currentSkinValue);

		//Debug.Log ("PlayerZone = ( " + playerZone.x + " / " + playerZone.y + " )");
		//Debug.Log ("SkinZone = ( " + valueZone.x + " / " + valueZone.y + " )");

		//player is not in the correct Zone
		if (playerZone.x != valueZone.x) {
			//if playerZoneNo is higher than valueZoneNo
			//that means player is below value -> move up
			if ((playerZone.x - valueZone.x) > 0)
				y = moveRate;
			//move down
			else if((playerZone.x - valueZone.x) < 0)
				y = -moveRate;
		}

		//player is in the correct zone, but not at the correct point
		else if ((playerZone.x == valueZone.x) && (playerZone.y != valueZone.y)) {
			//move up
			if ((playerZone.y - valueZone.y) > 0)
				y = moveRate;
			//move down
			else if((playerZone.y - valueZone.y) < 0)
				y = -moveRate;
		}

		rb.velocity = new Vector2(0, y);
		if (playerZone.x == valueZone.x) {
			CheckBounds (flowerZonePositions [(int) playerZone.x].y, flowerZonePositions [(int) playerZone.x].x); 
		}
		CheckBounds (lowerBoundScreen, upperBoundScreen);
	}
		


	private Vector2 ZoneOfPlayer(){
		Vector2 zoneOfPlayer = new Vector2(0, 0);
		float playerPos = this.transform.position.y;

		for (int i = 0; i < flowerZonePositions.Length; i++) {
			if (isValueInRange (playerPos, flowerZonePositions [i])) {
				zoneOfPlayer.x = i;
				break;
			}
		}		
		int currentZone = (int) zoneOfPlayer.x;
		float flowerZonePer = (flowerZonePositions [currentZone].x - flowerZonePositions [currentZone].y)/divider;
		zoneOfPlayer.y = Mathf.Floor( (playerPos - flowerZonePositions [currentZone].y) / flowerZonePer);
		return zoneOfPlayer;
	}

	private bool isValueInRange (float value, Vector2 range){
		return (value < range.x && value > range.y);
	}

	private bool isValueInRange (double value, Vector2double range){
		return (value < range.x && value > range.y);
	}



	//Add points if bee hits a flower
	private void OnTriggerEnter2D (Collider2D col){
		if (col.gameObject.tag == "Flower") {
			float yPos = col.gameObject.transform.position.y;
			int zone = flowerCreator.ReturnFlowerZone (yPos);
			int points = (int) Mathf.Pow(2, (zone));

			GameController.instance.ScoreChange (points);
			col.gameObject.GetComponentInChildren<ParticleSystem> ().Play ();
		}
	}

	private void CheckBounds(float lowerBound, float upperBound){
		float y = rb.transform.position.y;
		if (y > upperBound) {
			y = upperBound;
		} else if (y < lowerBound) {
			y = lowerBound;
		}
		rb.transform.position = new Vector2 (0f, y);
	}

	//Set the Bee-position to the transformed SkinValue
	private void BitalinoSetting(){
		float y = 0.0f;
		y = (float)SkinProcessor.instance.cleanedSkinValue;

		rb.transform.position = new Vector2 (0f, y);
		CheckBounds (lowerBoundScreen, upperBoundScreen);
	}

	struct Vector2double{
		public double x;
		public double y;

		public Vector2double(double n_x, double n_y){
			x = n_x;
			y = n_y;
		}
	}











	public void SetupSkinZones(){
		baselineAvg = SkinProcessor.instance.GetBaselineAvg();
		standardDev = SkinProcessor.instance.GetStandardDev();
		double max = SkinProcessor.instance.GetMax ();
		double min = SkinProcessor.instance.GetMin ();
		Debug.Log ("average: " + baselineAvg + "\n" + "standardDev: " + standardDev + "\n" + "max: " + max + "\n" + "min: " + min);

		double zoneHeight;
		if (standardDeviationAsZone) {
			zoneHeight = standardDev;
		} else {
			zoneHeight = (max - min) / SkinValueZones.Length;
		}

		SkinValueZones [0] = new Vector2double (baselineAvg + (3 * zoneHeight),	baselineAvg + (2 * zoneHeight))	;
		SkinValueZones [1] = new Vector2double (baselineAvg + (2 * zoneHeight),	baselineAvg + (1 * zoneHeight));
		SkinValueZones [2] = new Vector2double (baselineAvg + (1 * zoneHeight),	baselineAvg + (0 * zoneHeight));
		SkinValueZones [3] = new Vector2double (baselineAvg - (0 * zoneHeight),	baselineAvg - (1 * zoneHeight));
		SkinValueZones [4] = new Vector2double (baselineAvg - (1 * zoneHeight),	baselineAvg - (2 * zoneHeight));
		SkinValueZones [5] = new Vector2double (baselineAvg - (2 * zoneHeight),	baselineAvg - (3 * zoneHeight));
	}

	private Vector2 ZoneOfSkinValue (double currentSkinValue){
		Vector2 zoneOfSkinValue = new Vector2(-1 , -1);
		bool inZones = true;
		Debug.Log ("value: " + currentSkinValue); 

		for (int i = 0; i < SkinValueZones.Length; i++) {
			if (isValueInRange (currentSkinValue, SkinValueZones [i])) {
				zoneOfSkinValue.x = i;
				break;
			}
		}
		//if the current skinValue is higher than baselineAvg + 3 standardDev = set zone to 0
		//or                           lower than baselineAvg - 3 standardDev = set zone to 5
		if (zoneOfSkinValue.x == -1) {
			inZones = false;
			if (currentSkinValue > SkinValueZones [0].x) {
				zoneOfSkinValue.x = 0;
				zoneOfSkinValue.y = divider - 1;
			} else {
				zoneOfSkinValue.x = SkinValueZones.Length - 1;
				zoneOfSkinValue.y = 0;
			}
		}

		int currentZone = (int)zoneOfSkinValue.x;
		double zoneHeight = SkinValueZones [currentZone].x - SkinValueZones [currentZone].y;
		double standardDevPer = zoneHeight / divider;

		if (inZones) {
			//otherwise calculate the subZone
			//int currentZone = (int)zoneOfSkinValue.x;
			double temp = SkinValueZones [currentZone].y;
			int y = -1;

			if (currentSkinValue > temp)
				Debug.Log ("currentSkinValue > temp");
			while (currentSkinValue > temp) {
				y += 1;
				temp += standardDevPer;
			}
			zoneOfSkinValue.y = y;
		}

		return zoneOfSkinValue;
	}
}
