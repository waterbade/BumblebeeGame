using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinProcessor : SignalProcessor {
	//Singleton variables
	public static SkinProcessor instance; 
	private static SkinProcessor skinProcessor;

	public float diff = 0.1f;
	public double cleanedSkinValue;
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

	private Vector2double[] zoneArray = new Vector2double[6];
	private bool zonesSet = false;
	private Vector2 currentZone;

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
		max = -100;
		min = 100;
		currentZone = new Vector2 (0, 10);

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

	public Vector2 GetSkinValueZone(){
		return currentZone;
	}

	public override void ProcessSignals (double value, int i)
	{
		valueBuffer [i] = value;		//0

		//Cleanup the signal by filtering out jumps of more than 0.1 in value (and replacng them with the previous value
		if (i > 0) {
			smoothedBuffer [i] = CleanupSignal ((float)valueBuffer [i], (float)smoothedBuffer [i - 1]); //1
		} else {
			smoothedBuffer [i] = (double)Mathf.Min (Mathf.Max ((float)value, 0f), 10f);
		}

		//Smooth the cleaned signal
		//squaredBuffer [i] = Smooth(smoothedBuffer[i]); //2

		//do calculations at the right time
		if (EventManager.instance.setup) {
			//baselineValues.Add (squaredBuffer [i]);
			baselineValues.Add (smoothedBuffer [i]);
			_event = "setup";
		} else if (EventManager.instance.biofeedback) {
			//CalculateFeedbackAverage (squaredBuffer [i]); 
			CalculateFeedbackAverage (smoothedBuffer [i]); 
			_event = "feedback";}

		//save the cleaned value for other scripts to use
		//cleanedSkinValue = squaredBuffer [i];
		cleanedSkinValue = smoothedBuffer [i];

		//scale by using zones
		if (zonesSet) {
			currentZone = ZoneOfSkinValue (cleanedSkinValue);
			thresholdedBuffer [i] = ZoneToScreenPosition(currentZone); //3
			//Debug.Log("zone: "+currentZone + "screenPosition: " + thresholdedBuffer[i]);
			squaredBuffer[i] = Smooth(thresholdedBuffer[i]);

		} else{
			//thresholdedBuffer [i] = squaredBuffer [i];
			thresholdedBuffer [i] = smoothedBuffer [i];
		}

		//log it all for later evaluation
		LogSkinValuesToExcel (value, i);
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
		}
		float variance = (float) (sum / baselineValues.Count);
		standardDeviation = Mathf.Sqrt (variance);
		Debug.Log ("avg: " + baselineAvg + "\n standard dev: " + standardDeviation);
	}

	public void Restart(){
		baselineValues.Clear();
		baselineAvg = 0; 
		standardDeviation = 0;
		feedbackAvg = double.NaN;
		noOfFeedbackValues = 0f;
	}


	private void LogSkinValuesToExcel(double originalValue, int i){
		skinValues = ";"+originalValue.ToString () + ";" + smoothedBuffer[i].ToString ();
		skinValues = skinValues.Replace (".", ",");
	
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

	public void SetupSkinZones(){
		if (!zonesSet) {
			double zoneHeight = standardDeviation;

			zoneArray [0] = new Vector2double (baselineAvg + (3 * zoneHeight),	baselineAvg + (2 * zoneHeight));
			zoneArray [1] = new Vector2double (baselineAvg + (2 * zoneHeight),	baselineAvg + (1 * zoneHeight));
			zoneArray [2] = new Vector2double (baselineAvg + (1 * zoneHeight),	baselineAvg + (0 * zoneHeight));
			zoneArray [3] = new Vector2double (baselineAvg - (0 * zoneHeight),	baselineAvg - (1 * zoneHeight));
			zoneArray [4] = new Vector2double (baselineAvg - (1 * zoneHeight),	baselineAvg - (2 * zoneHeight));
			zoneArray [5] = new Vector2double (baselineAvg - (2 * zoneHeight),	baselineAvg - (3 * zoneHeight));

			zonesSet = true;
		}
	}

	private bool isValueInRange (double value, Vector2double range){
		return (value < range.x && value > range.y);
	}

	private Vector2 ZoneOfSkinValue (double value){
		Vector2 zone = new Vector2 (-1, -1);
		int divider = 10;

		//iterate through all zones and check if the value is in a zone
		for (int i = 0; i < zoneArray.Length; i++) {
			if (isValueInRange (value, zoneArray [i])) {
				zone.x = i;
				break;
			}
		}

		//if the current skinValue is not in a zone,
		if (zone.x == -1) {
			//if it is higher than baselineAvg + 3 standardDev = set zone to 0 and subzone to 10 (highest possible value)
			if (value > zoneArray [0].x) {
				zone.x = 0;
				zone.y = 0;

			// if current Value is lower than baselineAvg - 3 standardDev = set zone to 5
			} else {
				zone.x = zoneArray.Length - 1;
				//and subzone to 0 (lowest possible value)
				zone.y = 0;
			}
		}
		//otherwise if it is in a zone, calculate the subZone
		else {
			int currentZone = (int)zone.x;
			double zoneHeight = zoneArray [currentZone].x - zoneArray [currentZone].y;
			double standardDevPer = zoneHeight / divider;

			double temp = zoneArray [currentZone].y;
			int y = -1;

			while (value > temp) {
				y += 1;
				temp += standardDevPer;
			}
			zone.y = y;
		}

		return zone;
	}

	private double ZoneToScreenPosition(Vector2 zone){
		int screenHeight = 12;
		double zoneHeight = screenHeight / zoneArray.Length;
		double subZoneHeight = zoneHeight / 10;

		double screenPosition;

		int myZone = (int)zone.x;
		switch (myZone) {
		case 0:
			screenPosition = (2 * zoneHeight) + ((int)zone.y * subZoneHeight);
			break;
		case 1:
			screenPosition = (1 * zoneHeight) + ((int)zone.y * subZoneHeight);
			break;
		case 2:
			screenPosition = (0 * zoneHeight) + ((int)zone.y * subZoneHeight);
			break;
		case 3:
			screenPosition = (-1 * zoneHeight) + ((int)zone.y * subZoneHeight);
			break;
		case 4:
			screenPosition = (-2 * zoneHeight) + ((int)zone.y * subZoneHeight);
			break;
		case 5:
			screenPosition = (-3 * zoneHeight) + ((int)zone.y * subZoneHeight);
			break;
		default:
			screenPosition = screenHeight - subZoneHeight;
			Debug.Log ("skinValue out of zone");
			break;
		}

		return screenPosition;
	}



	private void LogZones(Vector2 zone){
		int _zone = (int) zone.x;
		Debug.Log (
		  "Zone: " + _zone + "\n"
		+ "subZone: " + zone.y + "\n"
		+ "currentSkinValue: " + cleanedSkinValue + "\n"
		+ "max: " + zoneArray [0].x + "\n"
		+ "min: " + zoneArray [5].y + "\n"
		+ "bottom: " + zoneArray [_zone].y + "\n" + "temp: " + zoneArray [_zone].y + "\n"
		+ "top: " + zoneArray [_zone].x
		);

	}


	struct Vector2double{
		public double x;
		public double y;

		public Vector2double(double n_x, double n_y){
			x = n_x;
			y = n_y;
		}
	}
}
