using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinProcessor : SignalProcessor {
	//Singleton variables
	public static SkinProcessor instance; 
	private static SkinProcessor skinProcessor;

	public float diff = 0.1f;
	public double currentSkinValue;
	public string skinValues = "";

	public string _event = "start";
	private string prevEvent = "";

	private List<double> baselineValues = new List<double> ();
	private double baselineAvg = 0; 
	private double standardDeviation = 0;
	private double feedbackAvg = double.NaN;
	private double noOfFeedbackValues = 0f;
	private double max;
	private double min;
	private double cameraSize;

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
		processorType = "SkinProcessor";
		valueBuffer = new double[bufferSize];
		smoothedBuffer = new double[bufferSize];
		squaredBuffer = new double[bufferSize];
		thresholdedBuffer = new double[bufferSize];
		cameraSize = Camera.main.orthographicSize;
		max = -100;
		min = 100;
	}
	
	public override void ProcessSignals (double value, int i)
	{
		valueBuffer [i] = value;		//0
		if (i > 0) {
			smoothedBuffer [i] = CleanupSignal ((float)valueBuffer [i], (float)smoothedBuffer [i - 1]); //1
		} else {
			smoothedBuffer [i] = (double)Mathf.Min (Mathf.Max ((float)value, 0f), 10f);
		}
		
		squaredBuffer [i] = Smooth(smoothedBuffer[i]); //2

		if (EventManager.instance.setup) {
			baselineValues.Add (squaredBuffer [i]);
			_event = "setup";
		} else if (EventManager.instance.biofeedback) {
			CalculateFeedbackAverage (squaredBuffer [i]); 
			_event = "feedback";}
		
		currentSkinValue = squaredBuffer [i];
		thresholdedBuffer [i] = squaredBuffer[i]; //ScaleEDA (squaredBuffer[i]);//3
		skinValues = ";"+value.ToString () + ";" + smoothedBuffer [i].ToString () + ";" + squaredBuffer [i].ToString () + ";" + thresholdedBuffer [i].ToString ();
		skinValues = skinValues.Replace (".", ",");
		SkinValuesString ();
	}
		
	private double CleanupSignal(float value, float previousValue){
		float difference = Mathf.Abs (value - previousValue);
		if (difference > diff) {
			return (double)previousValue;
		} else {
			return (double)value;}
	} 

	private void CalculateFeedbackAverage(double newValue){
		double newAvg;
		if (double.IsNaN (feedbackAvg)) {
			newAvg = newValue;
			CalculateBaselineAverage();
			CalculateStandardDeviation();
		} else
			newAvg = feedbackAvg * (noOfFeedbackValues / (noOfFeedbackValues + 1f)) + newValue / (noOfFeedbackValues + 1f);

		feedbackAvg = newAvg;
		noOfFeedbackValues += 1f;
		EventManager.instance.feedbackAverage = (float) feedbackAvg;
	}

	private void CalculateBaselineAverage (){
		double sum = 0;
		foreach (double d in baselineValues) {
			sum += d;
			if (d > max)
				max = d;
			else if (d < min)
				min = d;
		}

		baselineAvg = sum / baselineValues.Count;
		float f_baselineAvg = (float) baselineAvg;
		EventManager.instance.baselineAverage = f_baselineAvg;
	}

	private void CalculateStandardDeviation (){
		double sum = 0;
		foreach (double d in baselineValues) {
			double val = d - baselineAvg;
			double valSquared = val * val;
			sum += valSquared;
			//sum += Mathf.Pow ((val), 2);
		}
		float variance = (float) (sum / baselineValues.Count);
		standardDeviation = Mathf.Sqrt (variance);
		Debug.Log ("avg: " + baselineAvg + "\n standard dev: " + standardDeviation);
	}

	private double ScaleEDA (double oldValue){
		return oldValue;
	}

	public void Restart(){
		baselineValues.Clear();
		baselineAvg = 0; 
		standardDeviation = 0;
		feedbackAvg = double.NaN;
		noOfFeedbackValues = 0f;
	}

	//print values each time the event (start, setup, feedback) changes
	private void SkinValuesString(){
		//check if the event has changed
		if (!Equals(prevEvent, _event)){
			skinValues += ";" + _event +";";
			prevEvent = _event;
		}
		else
			skinValues += "; ;";

		string printedAvg = "";
		if ( Equals(_event, "feedback")){
			printedAvg = feedbackAvg.ToString();
		}
		else 
			printedAvg = baselineAvg.ToString();
		
		printedAvg = printedAvg.Replace (".", ",");
		skinValues += printedAvg;			
	}

	public double GetBaselineAvg(){
		return baselineAvg;
	}

	public double GetStandardDev(){
		return standardDeviation;
	}

	public double GetMax(){
		return max;
	}

	public double GetMin(){
		return min;
	}
}
