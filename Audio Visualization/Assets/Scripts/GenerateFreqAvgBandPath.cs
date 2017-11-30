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

		void Start () {
			lr = GetComponent<LineRenderer> ();
			lr.positionCount = 1;
			currPoint = new Vector3 (1, 0, 0);
			currRot = Quaternion.identity;
			currHeight = 5f;
			angleXZ = 0;
			angleY = 0;
			clampPos = 1;
			clampNeg = -1;
		}

		void Update () {
			if (frame / 10 <= numberOfPoints) {

				currPoint.y = 0;
				if (frame % 3 == 0) {
					float rotVal = 0;
					for (int i = 0; i < 8; i++) {
						float bandVal = AudioPeer._audioBand [i];
						float totalPercent = 1 / Mathf.Pow (2, AudioPeer._avgFreqBand - (i + 1) + 1);
						float direction = ((AudioPeer._audioBand [i] - AudioPeer._Amplitude) > 0 ? 1 : -1);
						rotVal += bandVal * totalPercent * direction;
					}

					if (rotVal > clampPos) rotVal = clampPos;
					else if (rotVal < clampNeg) rotVal = clampNeg;

					Debug.Log (rotVal + " " + clampPos + " " + clampNeg);

					angleXZ += rotVal;

					if (angleXZ > angleMax) {
						angleXZ = angleMax;
						clampNeg += -.1f;
					} else if (angleXZ < -angleMax) {
						angleXZ = -angleMax;
						clampPos += .1f;
					} else {
						clampNeg = (clampNeg * 3 + -1.0f) / 4f;
						clampPos = (clampPos * 3 + 1.0f) / 4f;
					}



					Debug.Log (angleXZ);

					currRot = Quaternion.Slerp (currRot, Quaternion.AngleAxis (angleXZ, Vector3.up * 10), .7f);
				}
				if (frame % 5 == 0) {

					angleY += .01f;
					currHeight = Mathf.Sin (angleY) * 10;

					currPoint += currRot * Vector3.Normalize (currPoint);
					currPoint.y = currHeight;
					lr.positionCount += 1;
					lr.SetPosition (lr.positionCount - 1, currPoint);

				}

				frame++;
			}
		}
	}
}
