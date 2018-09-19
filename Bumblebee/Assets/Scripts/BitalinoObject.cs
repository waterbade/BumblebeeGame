using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BitalinoObject : MonoBehaviour {
	public static BitalinoObject instance;
	private static BitalinoObject bitalino;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this.gameObject);
	}

	void Update(){
		if (Equals (SceneManager.GetActiveScene ().name, "end"))
			Destroy (gameObject);
	}
}
