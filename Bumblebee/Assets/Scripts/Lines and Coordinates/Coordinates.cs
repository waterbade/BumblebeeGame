using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : MonoBehaviour {

	private LineRenderer line;
	public float start_x = -7f;
	public float start_y = -4f;

	public float end_x = -7f;
	public float end_y = 6f;
	// Use this for initialization
	void Start () {
		line = (LineRenderer) this.GetComponent("LineRenderer");
		line.positionCount = 2;

		Vector3 start_point = new Vector3 (start_x, start_y);
		Vector3 end_point = new Vector3 (end_x, end_y);
		line.SetPosition (0, start_point);
		line.SetPosition (1, end_point);
	}
	
	// Update is called once per frame
	void Update () {
		/*Vector3 start_point = new Vector3 (start_x, start_y);
		Vector3 end_point = new Vector3 (end_x, end_y);
		line.SetPosition (0, start_point);
		line.SetPosition (1, end_point);*/
		
	}
}
