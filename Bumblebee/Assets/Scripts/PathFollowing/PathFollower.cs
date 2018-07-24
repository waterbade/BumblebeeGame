using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {

	public GameObject objectToMove;
	public float moveSpeed;
	private Node [] pathNodes;
	private float timer;
	private int currrentNode = 0;
	private static Vector3 currentPositonHolder;
	private Vector3 startPosition;

	void Awake(){
		pathNodes = GetComponentsInChildren<Node> ();
		foreach (Node n in pathNodes) {
			n.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
	// Use this for initialization
	void Start () {
		pathNodes = GetComponentsInChildren<Node> ();
		startPosition = pathNodes[0].transform.position;
		CheckNode ();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime * moveSpeed;
		if (objectToMove.transform.position != currentPositonHolder) {
			objectToMove.transform.position = Vector3.Lerp (startPosition, currentPositonHolder, timer);
		} else {
			if (currrentNode < pathNodes.Length - 1)
				currrentNode++;
			else {
				currrentNode = 0;
				objectToMove.transform.position = pathNodes [0].transform.position;
			}
			CheckNode ();
		}
	}

	void CheckNode(){
		timer = 0;
		startPosition = objectToMove.transform.position;
		currentPositonHolder = new Vector3 (pathNodes [currrentNode].transform.position.x, pathNodes [currrentNode].transform.position.y, -3) ;
	}
}
