using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour {
	public VideoPlayer vidPlayer;
	public VideoClip[] scaryVideos;

	void Start(){
		vidPlayer.loopPointReached += VideoHasPlayed;
		int videoNumber = SelectVideoClip (TrialManager.instance.trialNo);
		vidPlayer.clip = scaryVideos [videoNumber];
	}

	void Update(){
		if (EventManager.instance.playVideo)
		{		
			EventManager.instance.playVideo = false;
			vidPlayer.Play ();
		}
	}

	public void VideoHasPlayed(UnityEngine.Video.VideoPlayer vp){
		EventManager.instance.StartBiofeedback ();
	}

	private int SelectVideoClip(int trialNo){
		Debug.Log ("video selection started");
		List<int> possibleVideos = new List<int> ();
		switch (trialNo) {
		case 1:
			possibleVideos.Add (0);
			possibleVideos.Add (1);
			possibleVideos.Add (2);
			break;
		case 2:
			possibleVideos.Add (3);
			possibleVideos.Add (4);
			possibleVideos.Add (5);
			break;
		case 3:
			possibleVideos.Add (6);
			possibleVideos.Add (7);
			possibleVideos.Add (8);
			break;
		}

		List<int> videosPlayed = TrialManager.instance.GetVideosPlayed ();

		foreach (int playedVideo in videosPlayed) {
			if (possibleVideos.Contains(playedVideo))
				possibleVideos.Remove (playedVideo);
		}

		//select a random entry in the possble videos List
		int listEntryToPlay = Random.Range(0, possibleVideos.Count);
		Debug.Log ("listEntryToPlay is: " + listEntryToPlay);
		int videoToPlay = possibleVideos [listEntryToPlay];

		TrialManager.instance.SetVideosPlayed (videoToPlay);
		Debug.Log ("video selected = no. " + videoToPlay);
		return videoToPlay;
	}
}
