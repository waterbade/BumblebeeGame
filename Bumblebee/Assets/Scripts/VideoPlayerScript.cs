using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour {
	public VideoPlayer vidPlayer;
	//private bool playOnce = true;

	void Start(){
		vidPlayer.loopPointReached += VideoHasPlayed;
		Debug.Log ("added videoplayer function");
	}

	void Update(){
		if (EventManager.instance.playVideo)
		{		
			Debug.Log ("play video is true");	
			EventManager.instance.playVideo = false;
			vidPlayer.Play ();
			Debug.Log ("videoPlayer starts playing");
		}
	}

	public void VideoHasPlayed(UnityEngine.Video.VideoPlayer vp){
		Debug.Log ("video has played");
		EventManager.instance.StartBiofeedback ();
	}
}
