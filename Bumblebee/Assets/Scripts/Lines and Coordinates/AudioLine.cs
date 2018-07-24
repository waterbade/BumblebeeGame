using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLine : Line {

	private AudioSource audioSource;
	private bool play = true;

	void Awake () 
	{
		audioSource = GetComponent <AudioSource>();
	}

	void Update (){
		SetBuffer ("HeartProcessor");
		DrawProcessedLine ();
		PlayHeartbeat ();
	}

	private void PlayHeartbeat (){
		double lastVal = HeartProcessor.instance.GetBiosignalBuffer (3) [(reader.BufferSize - 1)];
		if (lastVal == 1 && play) {
			//Debug.Log ("play now");
			HeartProcessor.instance.CalculateHRV (Time.time);
			audioSource.Play ();
			play = false;
		} else if (lastVal == 0 && !play)
			play = true;
	}
}

