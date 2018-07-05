using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinProcessor : SignalProcessor {

	private bool isBaseline = false;
	private List<double> baselineBuffer = new List<double>();
	private double maximum = 0;
	private double minimum = 10;
	private double span = 0.0;

	public double windowsize = 10.0;
	public double max_Offset = 0.1;
	public Text minmaxText;

	private bool isExcitement;
	public Coordinates yCoordinates;

	// Use this for initialization
	void Start () {
		bufferSize = reader.BufferSize;
		processorType = "SkinProcessor";
		valueBuffer = new double[bufferSize];
		smoothedBuffer = new double[bufferSize];
		squaredBuffer = new double[bufferSize];
		thresholdedBuffer = new double[bufferSize];

		windowsize = Mathf.Abs( yCoordinates.start_y - yCoordinates.end_y);
		minmaxText.text = "min: " + minimum + "  max: " + maximum;
		
	}
	
	public override void ProcessSignals (double value, int i)
	{
		if (isBaseline) {
			baselineBuffer.Add (value);
			if ((value < minimum) && (value > - 5))
				minimum = value;
		}
		if (isExcitement){
			if ((value > maximum) && (value < 5))
				maximum = value;
			}
		valueBuffer [i] = value;
		smoothedBuffer [i] = value * 2;
		squaredBuffer [i] = ScaleEda(value);
		thresholdedBuffer [i] = value;

		Debug.Log ( "value: " + value + "  new value: " + squaredBuffer[i]);
	}

	public void ToggleBaseline()
	{
		isBaseline = isBaseline ? false : true;
	}
		
	public void CalculateMinMax() {
		float average = 0.0f;
		/*foreach (double d in baselineBuffer){
			if (d < minimum) 
				minimum = d;
			average += (float)d;
		}
		average = average / baselineBuffer.Count;*/
		maximum += max_Offset;
		minimum = (double) Mathf.Round ((float) minimum);
		maximum = (double) Mathf.Round ((float) maximum);
		span = maximum - minimum;
		minmaxText.text = "min: " + minimum + "  max: " + maximum;

	}

	public void ToggleExcitement()
	{
		isExcitement = isExcitement ? false : true;
	}
		
	private double ScaleEda (double value){
		//Debug.Log ("windowsize is " + windowsize);
		double newValue = ((value - minimum) / span ) * windowsize + yCoordinates.start_y;
		return newValue;
	}


}
