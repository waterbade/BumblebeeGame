using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public FlowerCreator flowerCreator;

	//movement variables
	public float speed = 0.0f;
	private float moveRate = 2.0f;

	//bat internal variables
	private Rigidbody2D rb;
	private SpriteRenderer beeRenderer;
	private Animator beeAnimator;


	void Start () {
		//set UI components
		// point counter

		//get Components of the Bat
		rb = GetComponent<Rigidbody2D> ();
		beeRenderer = this.GetComponent<SpriteRenderer> ();
		beeAnimator = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		MoveCharacter ();
	}

	private void MoveCharacter(){
		
		float move = Input.GetAxis ("Vertical");
		float y = 0.0f;
		if (move < 0)
			y = -moveRate;
		else if (move > 0)
			y = moveRate;
		
		rb.velocity = new Vector2 (speed, y);
	}

	/*private void OnCollisionEnter2D (Collision2D col){
		Debug.Log ("collison with " + col.gameObject.tag);
		if (col.gameObject.tag == "Flower") {
			EventManager.instance.ScoreChange (10);
			//increase points
		}
	}*/

	private void OnTriggerEnter2D (Collider2D col){
		if (col.gameObject.tag == "Flower") {
			float yPos = col.gameObject.transform.position.y;
			int zone = flowerCreator.ReturnFlowerZone (yPos);
			int points = (int) Mathf.Pow(2, (zone-1));

			GameController.instance.ScoreChange (points);
			//increase points
		}
	}
}
