using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;


public class TrialManager : MonoBehaviour {
	public static TrialManager instance;
	private static TrialManager trialControl;

	public string testPersonNumber;
	public int trialNo = 1;
	public string currentVisualization;
	public List<string> currentStrategy = new List<string>();

	private List<int> videosPlayed = new List<int>();

	public string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ;
	private StreamWriter sw;
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this.gameObject);

		dataPath += "\\" + GetFileName("scores") + ".txt";
	}

	public string GetFileName()
	{
		string fileName = "TP" + testPersonNumber + "_day" + trialNo.ToString() + "_" + currentVisualization + "_" + currentStrategy[0].ToString();
		return fileName;
	}

	private string GetFileName(string type)
	{
		string fileName = "TP" + testPersonNumber + "_day" + trialNo.ToString() + "_" + type;
		return fileName;
	}

	public List<int> GetVideosPlayed(){
		Debug.Log ("videos played: " + string.Join ("", new List<int> (videosPlayed).ConvertAll (i => i.ToString ()).ToArray ()));
		return videosPlayed;
	}

	public void SetVideosPlayed(int videoNo){
		videosPlayed.Add (videoNo);
	}

	public void NextStrategy(){
		if (currentStrategy.Count > 0)
			currentStrategy.RemoveAt (0);	
	}

	public void SaveScores (string baselineAvg, string feedbackAvg, string score)
	{
		try
		{
			if (sw == null)
			{ sw = File.AppendText (dataPath); }

			sw.WriteLine (currentVisualization + ", " + currentStrategy[0].ToString() + "\n baselineAvg = " + baselineAvg + "\n feedbackAvg = " + feedbackAvg + "\n score = " + score);
			sw.Flush();
		}
		catch (Exception e)
		{ UnityEngine.Debug.Log(e); }  
		NextStrategy ();
	}

	}