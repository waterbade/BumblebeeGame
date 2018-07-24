using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour {
	public VideoPlayer vidPlayer;
	private bool playOnce = true;

	void Start(){
		vidPlayer.loopPointReached += VideoHasPlayed;
	}
	void Update(){
		
		if (EventManager.instance.playVideo)
		//if (Time.realtimeSinceStartup > 5f && playOnce)
		{
			
			EventManager.instance.playVideo = false;
			vidPlayer.Play ();

			//playOnce = false;

		}
	}

	public void VideoHasPlayed(UnityEngine.Video.VideoPlayer vp){
		
		EventManager.instance.StartBiofeedback ();
		//vidPlayer.loopPointReached -= VideoHasPlayed;
	}
}
