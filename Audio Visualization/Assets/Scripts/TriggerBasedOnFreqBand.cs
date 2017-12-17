using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerBasedOnFreqBand : MonoBehaviour {

	public int _band;
	Image img;
	float lastBandVal = 0;
	public Color[] bandCols;

	void Start () { 
		img = this.GetComponent<Image> ();
		bandCols = new Color[8];
		bandCols [0] = new Color (1, 0, 0);
		bandCols [1] = new Color (1, .5f, 0);
		bandCols [2] = new Color (1, 1, 0);
		bandCols [3] = new Color (0, 1, 0);
		bandCols [4] = new Color (0, 0, 1);
		bandCols [5] = new Color (.3f, 0, .5f);
		bandCols [6] = new Color (.6f, 0, .8f);
		bandCols [7] = new Color (.65f, .16f, .16f);
	}

	void Update () {
		float bandVal = AudioPeer._audioBand [_band];

		if (bandVal - lastBandVal > .3) {
			img.color = bandCols [_band];
		} else if (bandVal - lastBandVal < 0) {
			img.color = Color.Lerp (img.color, Color.clear, (lastBandVal - bandVal) / bandVal);
		}

		lastBandVal = bandVal;
	}
}
