using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeckRacer {
	[RequireComponent (typeof(Terrain))]
	public class GenerateFreqBandTerrain : MonoBehaviour {
		Terrain terrain;
		TerrainData terrainData;
		int frameNumber;

		void Start () {
			terrain = GetComponent<Terrain> ();
			terrainData = terrain.terrainData;
			frameNumber = 0;
		}

		void Update () {
			float[,] heightMap = terrainData.GetHeights (0, frameNumber, 512, 1);
			for (int i = 0; i < 512; i++) {
				float ratio = (i % 64) / 64.0f;
				int idx = i / 64;
				if (idx < 7)
					heightMap [0, i] = ((AudioPeer._audioBand [idx] * (1 - ratio)) + (AudioPeer._audioBand [idx + 1] * ratio)) / 100;
				else
					heightMap [0, i] = AudioPeer._audioBand [idx] / 100;
			}
			terrainData.SetHeights (0, frameNumber, heightMap);

			frameNumber++;
		}
	}
}
