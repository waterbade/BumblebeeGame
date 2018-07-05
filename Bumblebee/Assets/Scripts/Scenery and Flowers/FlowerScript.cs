using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerScript : MonoBehaviour {

	private Collider2D blossomCollider; 
	private float blossomExtent;
	// Use this for initialization
	void Start () {
		blossomCollider = GetComponent <Collider2D>();
		blossomExtent = blossomCollider.bounds.extents.y;
		
	}

	public float GetColliderExtentY(){
		return blossomExtent;
	} 


}
