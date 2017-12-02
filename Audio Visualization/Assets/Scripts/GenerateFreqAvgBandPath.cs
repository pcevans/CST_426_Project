using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeckRacer {
	[RequireComponent (typeof(LineRenderer))]
	public class GenerateFreqAvgBandPath : MonoBehaviour {
		public GameObject trackPrefab;

		LineRenderer lr;
		Vector3 currPoint;
		Quaternion currRot;
		float currHeight;
		int frame = 0;
		public int numberOfPoints;
		public bool trackGenOn;
		public static Vector3[] trackPointArray = new Vector3[500];

		const float angleMax = 45;
		float angleXZ;
		float angleY;
		float clampNeg;
		float clampPos;
		bool biasPos, biasNeg;
		int lastIdx;
		float rotVal;

		void Start () {
			lr = GetComponent<LineRenderer> ();
			lr.positionCount = 1;
			currPoint = new Vector3 (1, 0, 0);
			currRot = Quaternion.identity;
			currHeight = 5f;
			angleXZ = 0;
			angleY = 0;
			rotVal = 0;
			clampPos = 1;
			clampNeg = -1;
			biasPos = false;
			biasNeg = false;
			lastIdx = 0;
		}

		float[] CalculatePercentages (int avgFreqBand, int numBands) {
			float[] percentages = new float[numBands];
			float sum = 0;
			for (int i = 0; i < 8; i++) {
				float weight = Mathf.Pow (2, Mathf.Abs (AudioPeer._avgFreqBand - i));//Mathf.Abs (avgFreqBand - i);// 
				percentages [i] = weight;
				sum += weight;
			}
			for (int i = 0; i < 8; i++) {
				percentages [i] /= sum;
			}
			return percentages;
		}

		void Update () {
			if (frame / 10 <= numberOfPoints) {

				currPoint.y = 0;
				if (frame % 3 == 0) {
					float[] percentages = CalculatePercentages (AudioPeer._avgFreqBand, AudioPeer._numBands);
					//Debug.Log (AudioPeer._Amplitude);
					float max = 0;
					int maxIdx = -1;
					for (int i = 0; i < 8; i++) {
						float bandVal = AudioPeer._audioBand [i];

						if (double.IsNaN (bandVal))
							bandVal = 0;

						if (bandVal >= max) {
							max = bandVal;
							maxIdx = i;
						}
//						float totalPercent = percentages [i];
//						int middle = AudioPeer._avgFreqBand;
//						float direction = (i - middle == 0 ? 0 : (i - middle > 0 ? 1 : -1));//((AudioPeer._audioBand [i] - AudioPeer._Amplitude) > 0 ? 1 : -1);
//						rotVal += bandVal * totalPercent * direction;
//						Debug.Log (i + " " + bandVal + " " + direction + " " + rotVal);
					}

					float direction = (maxIdx - AudioPeer._avgFreqBand == 0 ? 0 : (maxIdx - AudioPeer._avgFreqBand > 0 ? 1 : -1));//((AudioPeer._audioBand [i] - AudioPeer._Amplitude) > 0 ? 1 : -1);
					float nextVal = max * Mathf.Pow (2, Mathf.Abs (AudioPeer._avgFreqBand - maxIdx)) * direction;
					rotVal = (rotVal + nextVal) / 2;
					//rotVal /= 5;
					lastIdx = maxIdx;

//					if (rotVal > clampPos) {
//						rotVal = clampPos;
//						clampNeg += -.1f;
//						biasPos = true;
//						biasNeg = false;
//					} else if (rotVal < clampNeg) {
//						rotVal = clampNeg;
//						biasPos = false;
//						biasNeg = true;
//					}

					if (biasPos) clampNeg += -.1f;
					else if (biasNeg) clampPos += .1f;

					//clampNeg = (clampNeg * 9 + -1.0f) / 10f;
					//clampPos = (clampPos * 9 + 1.0f) / 10f;

					Debug.Log (rotVal + " " + max + " " + maxIdx);

					angleXZ += rotVal;

					Debug.Log (angleXZ);

					currRot = Quaternion.AngleAxis (angleXZ, Vector3.up);
				}
				if (frame % 5 == 0) {

					angleY += .01f;
					currHeight = Mathf.Sin (angleY) * 10;

					currPoint += currRot * Vector3.forward;
					currPoint.y = currHeight;
					lr.positionCount += 1;
					lr.SetPosition (lr.positionCount - 1, currPoint);

				}

				frame++;
			}
		}
	}
}
