using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeckRacer {
	public class TestAmplitude : MonoBehaviour {

		void Start () {
		}

		void Update () {
			transform.localScale = new Vector3 (AudioPeer._Amplitude, AudioPeer._Amplitude, AudioPeer._Amplitude);
		}
	}
}
