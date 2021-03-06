﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTrackRealtime : MonoBehaviour {
	public GameObject trackPrefab;
	public GameObject roadifier;
	public GameObject car;

	Vector3 lastCarPos;
	Vector3 currPoint;
	Quaternion currRot;
	float currHeight;
	int frame = 0;
	List<Vector3> trackPoints = new List<Vector3> ();
	List<GameObject> trackSegments = new List<GameObject> ();
	int closestTrackIdx;
	int lastVisibleTrackIdx;
	Roadifier r;
	//private Vector3[] trackPointArray = new Vector3[1000];

	const float angleMax = 45;
	float angleXZ;
	float angleY;
	float clampNeg;
	float clampPos;
	bool biasPos, biasNeg;
	int lastIdx;
	float rotVal;
	bool runOnce;

	void Start () {
		lastVisibleTrackIdx = 0;
		closestTrackIdx = 0;
		r = roadifier.GetComponent<Roadifier> ();
		lastCarPos = car.transform.position;
		currPoint = new Vector3 (0, 0, 0);
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
		runOnce = true;
	}

	float[] CalculatePercentages (int avgFreqBand, int numBands) {
		float[] percentages = new float[numBands];
		float sum = 0;
		for (int i = 0; i < 8; i++) {
			float weight = Mathf.Pow (2, Mathf.Abs (AudioPeer._avgFreqBand - i));//Mathf.Abs (avgFreqBand - i);
			percentages [i] = weight;
			sum += weight;
		}
		for (int i = 0; i < 8; i++) {
			percentages [i] /= sum;
		}
		return percentages;
	}

	void Update () {
		float amtMoved = Vector3.Magnitude (car.transform.position - lastCarPos);
		if (amtMoved < .5f || Vector3.Magnitude (trackPoints [trackPoints.Count - 1] - car.transform.position) > 60f)
			amtMoved = .5f;
		if (amtMoved > .000001) {
			currPoint.y = 0;
			if (frame % 3 == 0) {
				//float[] percentages = CalculatePercentages (AudioPeer._avgFreqBand, AudioPeer._numBands);
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
					//float totalPercent = percentages [i];
					//int middle = AudioPeer._avgFreqBand;
					//float direction = (i - middle == 0 ? 0 : (i - middle > 0 ? 1 : -1));//((AudioPeer._audioBand [i] - AudioPeer._Amplitude) > 0 ? 1 : -1);
					//rotVal += bandVal * totalPercent * direction;
				}

				float direction = (maxIdx - AudioPeer._avgFreqBand == 0 ? 0 : (maxIdx - AudioPeer._avgFreqBand > 0 ? 1 : -1));//((AudioPeer._audioBand [i] - AudioPeer._Amplitude) > 0 ? 1 : -1);
				float nextVal = max * Mathf.Pow (2, Mathf.Abs (AudioPeer._avgFreqBand - maxIdx)) * direction * (amtMoved * 2);

				rotVal = (rotVal + nextVal) / 2;
				//rotVal /= 5;
				lastIdx = maxIdx;

				//if (rotVal > clampPos) {
				//	rotVal = clampPos;
				//	clampNeg += -.1f;
				//	biasPos = true;
				//	biasNeg = false;
				//} else if (rotVal < clampNeg) {
				//	rotVal = clampNeg;
				//	biasPos = false;
				//	biasNeg = true;
				//}

				//if (biasPos) clampNeg += -.1f;
				//else if (biasNeg) clampPos += .1f;

				//clampNeg = (clampNeg * 9 + -1.0f) / 10f;
				//clampPos = (clampPos * 9 + 1.0f) / 10f;

				if (rotVal > 5)
					rotVal = 5;

				//angleXZ += rotVal;

				if (trackPoints.Count > 10) {
					angleXZ += rotVal;
					currRot = Quaternion.AngleAxis (angleXZ, Vector3.up);
				}

			}
			if (frame % 6 == 0) {
				float multiplier = car.GetComponent<Rigidbody> ().velocity.magnitude / 10;
				multiplier = amtMoved;
				//angleY += .01f * multiplier;
				// currHeight = Mathf.Sin(angleY) * 10;
				currPoint += currRot * (Vector3.forward * multiplier);
				//currPoint.y = currHeight;
				trackPoints.Add (currPoint);

				const int posCount = 1;
				if (trackPoints.Count % posCount == 0 && trackPoints.Count >= 3) {
					GameObject obj = r.GenerateRoad (trackPoints.GetRange (trackPoints.Count - 3, posCount + 2));
					trackSegments.Add (obj);
					//if (trackPoints.Count > 50)
					//obj.SetActive (false);
					
				}

				lastCarPos = car.transform.position;


				int oldClosestTrackIdx = closestTrackIdx;
				//Debug.Log (Vector3.Magnitude (car.transform.position - trackPoints [closestTrackIdx + 1]) + " " + Vector3.Magnitude (car.transform.position - trackPoints [closestTrackIdx]));
				while (closestTrackIdx < trackPoints.Count - 1 && Vector3.Magnitude (car.transform.position - trackPoints [closestTrackIdx + 1]) < Vector3.Magnitude (car.transform.position - trackPoints [closestTrackIdx])) {
					closestTrackIdx++;
				}
				while (closestTrackIdx > 1 && Vector3.Magnitude (car.transform.position - trackPoints [closestTrackIdx - 1]) < Vector3.Magnitude (car.transform.position - trackPoints [closestTrackIdx])) {
					closestTrackIdx--;
				}

				//Debug.Log (closestTrackIdx);

				int viewWindow = 50;

				bool frontCheck = (oldClosestTrackIdx >= viewWindow && closestTrackIdx >= viewWindow);
				bool backCheck = (oldClosestTrackIdx + viewWindow <= trackSegments.Count - 1 && closestTrackIdx + viewWindow <= trackSegments.Count - 1);

				if (trackSegments.Count > 0 && trackSegments.Count > closestTrackIdx + viewWindow) {
					for (int i = trackSegments.Count - 1; i > closestTrackIdx + viewWindow; i--)
						trackSegments [trackSegments.Count - 1].SetActive (false);
					//Debug.Log ("Deactivate " + (trackSegments.Count - 1));
				}

				int dir = (closestTrackIdx - oldClosestTrackIdx >= 0 ? 1 : -1);
				if ((dir == 1 && frontCheck) || (dir == -1 & backCheck)) {
					for (int i = oldClosestTrackIdx - dir * viewWindow; i != closestTrackIdx - dir * viewWindow; i += dir) {
						trackSegments [i].SetActive (false);
						//Debug.Log ("Deactivate " + i);
					}
					trackSegments [closestTrackIdx - dir * viewWindow].SetActive (false);
				}

				if ((dir == 1 && backCheck) || (dir == -1 & frontCheck)) {
					for (int i = oldClosestTrackIdx + dir * viewWindow; i != closestTrackIdx + dir * viewWindow; i += dir) {
						trackSegments [i].SetActive (true);

						Debug.Log ("Activate " + i);
					}
					trackSegments [closestTrackIdx + dir * viewWindow].SetActive (true);
					lastVisibleTrackIdx = closestTrackIdx + dir * viewWindow;
					Debug.Log ("Activate " + (closestTrackIdx + dir * viewWindow));
				}

				int limit = Mathf.Min (trackSegments.Count - 1, closestTrackIdx + viewWindow);
				for (int i = lastVisibleTrackIdx; i <= limit; i++) {
					trackSegments [i].SetActive (true);
				}
				lastVisibleTrackIdx = limit;
				
			}


			frame++;
		}
	}
}
