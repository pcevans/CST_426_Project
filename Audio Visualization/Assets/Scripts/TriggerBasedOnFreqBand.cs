using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBasedOnFreqBand : MonoBehaviour
{
    public GameObject debugVisualizer;

    public int _band;
    int frameNumber;
    float lastBandVal = 0;

    void Start()
    { 
        frameNumber = 0;
    }

    void Update()
    {
        float bandVal = AudioPeer._audioBand[_band];
        if (bandVal - lastBandVal > .4)
            Debug.Log(bandVal);

        lastBandVal = bandVal;
        frameNumber++;
    }
}
