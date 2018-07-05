using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MuscleProcessor : SignalProcessor {

	public double noMuscleMovement = 0.8;
	public double lowMuscleMovement;

	void Start(){
		bufferSize = reader.BufferSize;
		processorType = "MuscleProcessor";
		valueBuffer = new double[bufferSize];
		smoothedBuffer = new double[bufferSize];
		squaredBuffer = new double[bufferSize];
		thresholdedBuffer = new double[bufferSize];
	}
	public override void ProcessSignals (double value, int i) {
		valueBuffer [i] = value;
		value = MakePositive (value) *50.0;
		squaredBuffer[i] = value; //Square (value);
		smoothedBuffer [i] = Smooth (squaredBuffer [i]);
		thresholdedBuffer [i] = Threshold (smoothedBuffer [i], noMuscleMovement, lowMuscleMovement);
	}
}
