using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public FlowerCreator flowerCreator;
	private BitalinoReader bitalinoReader;
	//movement variables
	private float moveRate = 2.0f;
	private Rigidbody2D rb;
	private float upperBound;
	private float lowerBound;
	private float halfBee; 
	void Start () {
		
		rb = GetComponent<Rigidbody2D> ();
		halfBee = this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
		upperBound = Camera.main.orthographicSize - halfBee;
		lowerBound = -Camera.main.orthographicSize + halfBee;

		bitalinoReader = BitalinoObject.instance.GetComponentInChildren<BitalinoReader> ();
		//beeRenderer = this.GetComponent<SpriteRenderer> ();
		//beeAnimator = this.GetComponent<Animator> ();
	}

	// Update is called once per frame
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

	private void OnTriggerEnter2D (Collider2D col){
		if (col.gameObject.tag == "Flower") {
			float yPos = col.gameObject.transform.position.y;
			int zone = flowerCreator.ReturnFlowerZone (yPos);
			int points = (int) Mathf.Pow(2, (zone));

			GameController.instance.ScoreChange (points);
			col.gameObject.GetComponentInChildren<ParticleSystem> ().Play ();
		}
	}

	private void CheckBounds(){
		float y = rb.transform.position.y;
		if (y > upperBound)
			y = upperBound;
		else if (y < lowerBound)
			y = lowerBound;
		rb.transform.position = new Vector2 (0f, y);
	}

	private void BitalinoMovement(){
		float y = 0.0f;
		y = (float) SkinProcessor.instance.GetCurrentValue();

		rb.transform.position = new Vector2 (0f, y);
		CheckBounds ();
	}

	private void KeyboardMovement(){
		
		float y = 0.0f;
		float move = Input.GetAxis ("Vertical");

		if (move < 0)
			y = -moveRate;
		else if (move > 0)
			y = moveRate;
	
		rb.velocity = new Vector2(0, y);
		CheckBounds ();

	}
}
