using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartProcessor : SignalProcessor {
	//Singleton variables
	public static HeartProcessor instance; 
	private static HeartProcessor heartProcessor;

	public double heartbeatThreshold = 6.0;
	//Calculating the Heart Rate Varability HRV
	private float lastPeakTime = 0.0f; 
	private float lastRRinterval = 0.0f;
	private float RMSSD = 0.0f; //  RMSSD. This is the Root Mean Square of Successive Differences between each heartbeat
	private List <float> SuccessiveIntervalList = new List<float>(); 
	public Text HRVtext;

	void Awake()
	{
		//SingletonPattern
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		bufferSize = reader.BufferSize;
		processorType = "HeartProcessor";
		valueBuffer = new double[bufferSize];
		smoothedBuffer = new double[bufferSize];
		squaredBuffer = new double[bufferSize];
		thresholdedBuffer = new double[bufferSize];
	}

	public override void ProcessSignals (double value, int i)
	{
		valueBuffer [i] = value;
		smoothedBuffer [i] = Smooth (value);
		//Debug.Log ("smoothedBuffer: " + smoothedBuffer[i]);
		squaredBuffer [i] = Square(smoothedBuffer [i]);
		thresholdedBuffer [i] = Threshold ((double)squaredBuffer [i], heartbeatThreshold);
	}

	public void CalculateHRV(float peakTime) {
		//calculate the interval between two R-peaks
		float RRinterval = (peakTime - lastPeakTime);
		//remeber the time of the last peak
		lastPeakTime = peakTime;
		//square the difference between two intervals
		float SuccessiveInterval = Mathf.Pow ((RRinterval - lastRRinterval), 2);
		SuccessiveIntervalList.Add (SuccessiveInterval);
		float addedUpRRIntervals = 0.0f;

		foreach (float f in SuccessiveIntervalList) {
			addedUpRRIntervals += SuccessiveInterval;
		}
		RMSSD = (Mathf.Sqrt (addedUpRRIntervals/SuccessiveIntervalList.Count));
		HRVtext.text = "HRV : " + RMSSD;

		if (SuccessiveIntervalList.Count > 200)
			SuccessiveIntervalList.RemoveAt (0);
	}
}
