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

		float angle;

		void Start () {
			lr = GetComponent<LineRenderer> ();
			lr.positionCount = 1;
			currPoint = new Vector3 (1, 0, 0);
			currRot = Quaternion.identity;
			currHeight = 5f;

		}

		void Update () {
			if (frame / 10 <= numberOfPoints) {

				currPoint.y = 0;
				if (frame % 3 == 0) {
					float rotVal = 0;

					for (int i = 0; i < 8; i++) {
						float bandVal = (i - AudioPeer._avgFreqBand) * AudioPeer._audioBand [i];
						rotVal += bandVal;
					}
					if (!(rotVal > 0 || rotVal < 0))
						rotVal = 0;
					rotVal -= 6;
					rotVal *= 5;

					currRot = Quaternion.Slerp (currRot, currRot * Quaternion.AngleAxis (rotVal, Vector3.up * 10), .7f);
				}
				if (frame % 5 == 0) {

					angle += .01f;
					currHeight = Mathf.Sin (angle) * 10;

					currPoint += currRot * Vector3.Normalize (currPoint);
					currPoint.y = currHeight;
					lr.positionCount += 1;
					lr.SetPosition (lr.positionCount - 1, currPoint);

				}
				if (frame % 10 == 0) {
					if (trackGenOn) {
						GameObject newTrack = Instantiate (trackPrefab, currPoint + lr.transform.position, currRot);
						newTrack.name = "Track piece " + (frame / 10);
					}
					trackPointArray [frame / 10] = currPoint;
				}
			
				if (frame % 100 == 0) {
					currRot = Quaternion.identity;
				}
		

				frame++;
			}
		}
	}
}
