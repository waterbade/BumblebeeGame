// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {
    public BitalinoReader reader;
	public MuscleProcessor muscleProcessor;
	public HeartProcessor heartProcessor;
	public SkinProcessor skinProcessor;

	public int channelOrStep = 0;
	public bool isProcessedSignal = false;
	public string processorType = "HeartProcessor";
	public float offset = 2.0f;

	private LineRenderer line;
	private double divisor = 1;
	private double[] bufferToUse;

	void Start () {
        line = (LineRenderer) this.GetComponent("LineRenderer");
        line.positionCount = reader.BufferSize;
		bufferToUse = new double[reader.BufferSize];
	}
		
	/// Draw the new point of the line
	void Update () {
		SetBuffer (processorType);
		// asStart returns a boolean which is set to false from the beginning and OnApplicationQuit()
		if (reader.asStart) {
			if (!isProcessedSignal) {
				DrawUnprocessedLine ();
			} else if (isProcessedSignal) {
				DrawProcessedLine ();
			}
		}
	}

	protected void SetBuffer (string processorType){
		if (Equals (processorType, heartProcessor.GetProcesserType ())) {
			bufferToUse = (heartProcessor.GetBiosignalBuffer (channelOrStep));
		} else if (Equals (processorType, muscleProcessor.GetProcesserType ())) {
			bufferToUse = muscleProcessor.GetBiosignalBuffer (channelOrStep);
		} else if (Equals (processorType, skinProcessor.GetProcesserType ())) {
			bufferToUse = skinProcessor.GetBiosignalBuffer(channelOrStep);
		}
	}

	protected void DrawUnprocessedLine(){
		int i = 0;
		foreach (BITalinoFrame f in reader.getBuffer()) {
			//float posX = (float) (-7.5f+15f*((1.0/reader.BufferSize)*i));
			float posX = (float)(15f * ((1.0 / reader.BufferSize) * i) - 7.5f);
			float posY = (float)((f.GetAnalogValue (channelOrStep)) / divisor);
			line.SetPosition (i, new Vector3 (posX, posY, 0));
			i++;
		}
	}

	protected void DrawProcessedLine(){
		int i = 0;
		//if (Equals (processorType, heartProcessor.GetProcesserType ())) {
		foreach (double d in bufferToUse) {
				//UnityEngine.Debug.Log ("d: " + d);
				float posX = (float)(15f * ((1.0 / reader.BufferSize) * i) - 7.5f);
				float posY = (float)(d / divisor) + offset;
				line.SetPosition (i, new Vector3 (posX, posY, 0));
				//UnityEngine.Debug.Log ("X: " + posX + " Y: " + posY);
				i++;
			}
	}
}

