using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinProcessor : SignalProcessor {
	//Singleton variables
	public static SkinProcessor instance; 
	private static SkinProcessor skinProcessor;
	[SerializeField]
	private float scaleMin = 1.0f;
	[SerializeField]
	private float scaleMax = 1.5f;

	public float cutOffMax = 10f;
	public float cutOffMin = 0f;

	public bool calculateMinMax;

	private double feedbackAvg = double.NaN; 
	private double noOfFeedbackValues = 0f;

	private double baselineAvg = double.NaN; 
	private double noOfBaselineValues = 0f;

	private double cameraSize;

	private double currentSkinValue;

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
	}
	
	public override void ProcessSignals (double value, int i)
	{
		
		valueBuffer [i] = value;		//0

		if (i > 0)
			smoothedBuffer [i] = CleanupSignal ((float)value, (float)smoothedBuffer [i - 1], i);//1
		else
			smoothedBuffer [i] = (double) Mathf.Min(Mathf.Max( (float) value, cutOffMin), cutOffMax);
		
		squaredBuffer [i] = Smooth(smoothedBuffer[i]); //2
		
		thresholdedBuffer [i] =  ScaleEDA (squaredBuffer[i]);//3

		currentSkinValue = thresholdedBuffer [i];
		Debug.Log("value = " + value + " \n processed: " + currentSkinValue);

		if (EventManager.instance.setup) {
			CalculateBaselineAverage (squaredBuffer [i]);
		}
		else if (EventManager.instance.biofeedback){
			CalculateFeedbackAverage (squaredBuffer [i]);
		}
	}
		
	public void CalculateMinMax(float d) {
		scaleMax = Mathf.Max (d, scaleMax);
		scaleMin = Mathf.Min (d, scaleMin);
	}

	private double CleanupSignal(float value, float previousValue, int i){
		double max = Mathf.Max (value, previousValue);
		double min = Mathf.Min (value, previousValue);

		if (max - min > 1)
			return (double)Mathf.Min (Mathf.Max (previousValue, cutOffMin), cutOffMax);
		else
			return (double)Mathf.Min (Mathf.Max (value, cutOffMin), cutOffMax);
	} 

	public double GetCurrentValue(){
		return currentSkinValue;
	}

	private double ScaleEDA (double value){
		double span = scaleMax - scaleMin;
		double newValue = ((value - scaleMin) / span) * (cameraSize * 2) - cameraSize;
		return newValue;
		
	}

	/* new = a1*(n/(n+1)) + a2/(n+1)
		 * 
		 * a1 = feedbackAverage
		 * a2 = newValue
		 * n = number of values until now */

	private void CalculateBaselineAverage(double newValue){
		double newAvg;
		if (double.IsNaN (baselineAvg))
			newAvg = newValue;
		else
			newAvg = baselineAvg * (noOfBaselineValues / (noOfBaselineValues + 1f)) + newValue / (noOfBaselineValues + 1f);

		baselineAvg = newAvg;
		noOfBaselineValues += 1f;
		float f_baselineAvg = (float) baselineAvg;
		EventManager.instance.baselineAverage = f_baselineAvg;

		CalculateMinMax ((float) newValue);
	}

	private void CalculateFeedbackAverage(double newValue){
		double newAvg;
		if (double.IsNaN (feedbackAvg)) {
			newAvg = newValue;
			RearrangeMinMax ();
		}
		else
			newAvg = feedbackAvg * (noOfFeedbackValues / (noOfFeedbackValues + 1f)) + newValue / (noOfFeedbackValues + 1f);

		feedbackAvg = newAvg;
		noOfFeedbackValues += 1f;
		EventManager.instance.feedbackAverage = (float) feedbackAvg;
	}

	public void Restart(){
		scaleMax = 0f;
		scaleMin = 100f;
	}

	private void RearrangeMinMax (){
		scaleMax = Mathf.Min ((float)baselineAvg + 1f, scaleMax);
		scaleMin = Mathf.Max ((float)baselineAvg - 1f, scaleMin);
	}
}
