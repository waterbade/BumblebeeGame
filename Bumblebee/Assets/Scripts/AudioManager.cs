using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip[] scarySounds;
	public AudioClip[] bgMusic;

	public AudioSource scarySoundSource;
	public AudioSource musicSource;

	private List<int> soundsPlayed = new List<int> (); 
	private bool soundSelected = false;

	public static AudioManager instance;
	private static AudioManager audioManager;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this.gameObject);
	}

	
	// Update is called once per frame
	void Update () {
		if (!soundSelected) {
			int selectedSound = SelectAudioClip (TrialManager.instance.trialNo);
			AudioClip sound = scarySounds [selectedSound];
			scarySoundSource.clip = sound;
			soundSelected = true;
		}		
	}

	private int SelectAudioClip(int trialNo){
		Debug.Log ("sound selection started");
		List<int> possibleSounds = new List<int> ();
		switch (trialNo) {
		case 1:
			possibleSounds.Add (0);
			possibleSounds.Add (1);
			possibleSounds.Add (2);
			break;
		case 2:
			possibleSounds.Add (3);
			possibleSounds.Add (4);
			possibleSounds.Add (5);
			break;
		case 3:
			possibleSounds.Add (6);
			possibleSounds.Add (7);
			possibleSounds.Add (8);
			break;
		}

		foreach (int playedSound in soundsPlayed) {
			if (possibleSounds.Contains(playedSound))
				possibleSounds.Remove (playedSound);
		}

		//select a random entry in the possible sounds List
		int listEntryToPlay = Random.Range(0, possibleSounds.Count);
		Debug.Log ("listEntryToPlay is: " + listEntryToPlay);
		int audioclipToPlay = possibleSounds [listEntryToPlay];

		soundsPlayed.Add (audioclipToPlay);
		Debug.Log ("sounds selected = set no. " + audioclipToPlay);
		return audioclipToPlay;
	}

	public void ResetSounds(){
		soundSelected = false;
	}

	public void StopMusic(){
		musicSource.Stop ();
		Debug.Log ("sop music");
	}

	public void playVideoSounds(){
		scarySoundSource.Play ();
		Debug.Log ("play sounds");
	}

	public void ResumeMusic(){
		scarySoundSource.Stop ();
		musicSource.volume = 0.0f;
		musicSource.Play ();
		StartCoroutine (FadeInMusic());
		Debug.Log ("resume music");

	}

	IEnumerator FadeInMusic(){
		while (musicSource.volume <= 1.0f) {
			musicSource.volume += 0.025f;
			yield return new WaitForSeconds (0.5f);
		}
	}
}
