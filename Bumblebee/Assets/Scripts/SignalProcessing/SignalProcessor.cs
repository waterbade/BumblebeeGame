using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SignalProcessor : MonoBehaviour {

	public BitalinoReader reader;
	//private int muscleChannel = 0;
	//private int heartChannel = 1;
	protected int bufferSize;

	protected string processorType;
	protected double[] valueBuffer;
	protected double[] smoothedBuffer;
	protected double[] squaredBuffer;
	protected double[] thresholdedBuffer;

	protected List<double> SmoothList = new List<double>();
	protected int Listlength = 10;

	// Use this for initialization
	void Start () {
		bufferSize = reader.BufferSize;
		processorType = "SignalProcessor";
		valueBuffer = new double[bufferSize];
		smoothedBuffer = new double[bufferSize];
		squaredBuffer = new double[bufferSize];
		thresholdedBuffer = new double[bufferSize];
	}

	public void PushBuffers (int i){
		valueBuffer [i] = valueBuffer [i + 1];
		smoothedBuffer [i] = smoothedBuffer [i + 1];
		squaredBuffer [i] = squaredBuffer [i + 1];
		thresholdedBuffer [i] = thresholdedBuffer [i + 1];
	}

	public virtual void ProcessSignals (double value, int i)
	{}

	protected void AddToList (double val, List<double> list, int listLength) {
		list.Add (val);
		if (list.Count > listLength) 
			list.RemoveAt (0);
	}

	protected double Smooth (double val){
		//Debug.Log ("processorType:" + processorType + "  smoothList: "+ string.Join("", new List<double>(SmoothList).ConvertAll(i => i.ToString()).ToArray()));
		List<double> smoothList = new List<double>(); 
		AddToList (val, SmoothList, Listlength);
		smoothList = SmoothList;
		double avg = 0;
		foreach (double d in smoothList) {
			avg += d;
		}
		return avg = avg / smoothList.Count;
	}

	protected double Square (double val) {
		return (Mathf.Pow ((float)val, 2)); 
	}

	protected double MakePositive (double val) {
		if (Mathf.Sign ((float) val) == -1)
			val *= -1; 
		return (double) val;
	}

	protected double Threshold (double avg, double threshold) {
		if (avg < threshold)
			return 0;
		else
			return 1;
	}

	protected double Threshold (double avg, double threshold1, double threshold2) {
		if (avg < threshold1)
			return 0;
		else if (avg < threshold2)
			return 0.5;
		else
			return 1;
	}
		
	public double[] GetBiosignalBuffer(int step){
		switch (step) {
		case 0:
			return valueBuffer;
			break;
		case 1: 
			return smoothedBuffer;
			break;
		case 2:
			return squaredBuffer;
			break;
		case 3:
			return thresholdedBuffer;
			break;
		default:
			return smoothedBuffer;
		}
	}

	public string GetProcesserType(){
		return processorType;
	}

}
