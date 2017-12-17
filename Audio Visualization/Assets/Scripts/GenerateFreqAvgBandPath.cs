using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class GenerateFreqAvgBandPath : MonoBehaviour {
	public GameObject trackPrefab;
	public GameObject roadifier;
	public GameObject car;

	LineRenderer lr;
    Vector3 lastCarPos;
	Vector3 currPoint;
	Quaternion currRot;
	float currHeight;
	int frame = 0;
	public int numberOfPoints;
	public bool trackGenOn;
	private Vector3[] trackPointArray = new Vector3[1000];

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
		lr = GetComponent<LineRenderer> ();
		lr.positionCount = 1;
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
        float amtMoved = Vector3.Magnitude(car.transform.position - lastCarPos);
        if (amtMoved < .5f || Vector3.Magnitude(lr.GetPosition(lr.positionCount - 1) - car.transform.position) > 60f) 
            amtMoved = .5f;
        //if (lr.positionCount < numberOfPoints) {
        if (amtMoved > .000001)
        {
            currPoint.y = 0;
            if (frame % 3 == 0)
            {
                //float[] percentages = CalculatePercentages (AudioPeer._avgFreqBand, AudioPeer._numBands);
                //Debug.Log (AudioPeer._Amplitude);
                float max = 0;
                int maxIdx = -1;
                for (int i = 0; i < 8; i++)
                {
                    float bandVal = AudioPeer._audioBand[i];

                    if (double.IsNaN(bandVal))
                        bandVal = 0;

                    if (bandVal >= max)
                    {
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
                float nextVal = max * Mathf.Pow(2, Mathf.Abs(AudioPeer._avgFreqBand - maxIdx)) * direction * (amtMoved * 2);
                Debug.Log(nextVal);
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

                //			if (biasPos) clampNeg += -.1f;
                //			else if (biasNeg) clampPos += .1f;

                //clampNeg = (clampNeg * 9 + -1.0f) / 10f;
                //clampPos = (clampPos * 9 + 1.0f) / 10f;

                //Debug.Log (rotVal + " " + max + " " + maxIdx);

                if (rotVal > 5)
                    rotVal = 5;

                //angleXZ += rotVal;

                //Debug.Log (angleXZ);

                if (lr.positionCount > 10)
                {
                    angleXZ += rotVal;
                    currRot = Quaternion.AngleAxis(angleXZ, Vector3.up);
                }

            }
            if (frame % 5 == 0)
            {
                float multiplier = car.GetComponent<Rigidbody>().velocity.magnitude / 10;
                multiplier = amtMoved;
                angleY += .01f * multiplier;
                currHeight = Mathf.Sin(angleY) * 10;

                //Debug.Log(amtMoved);
                currPoint += currRot * (Vector3.forward * multiplier);
                currPoint.y = currHeight;
                lr.positionCount += 1;
                lr.SetPosition(lr.positionCount - 1, currPoint);

                const int posCount = 1;
                if (lr.positionCount % posCount == 0)
                {
                    lr.GetPositions(trackPointArray);
                    int numPositions = lr.positionCount;
                    List<Vector3> l = new List<Vector3>();
                    int forCount = (numPositions < posCount + 2 ? numPositions : posCount + 2);
                    for (int i = 0; i < forCount; i++)
                        l.Add(trackPointArray[numPositions - i - 1]);
                    Roadifier r = roadifier.GetComponent<Roadifier>();
                    r.GenerateRoad(l);
                }

                lastCarPos = car.transform.position;
            }

            frame++;
        }
	}
}
