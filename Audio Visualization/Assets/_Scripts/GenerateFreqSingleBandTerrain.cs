using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Terrain))]
public class GenerateFreqSingleBandTerrain : MonoBehaviour {
	Terrain terrain;
	TerrainData terrainData;
	public int _band;
	int frameNumber;

	void Start () {
		terrain = GetComponent<Terrain> ();
		terrainData = terrain.terrainData;
		frameNumber = 0;
	}

	void Update () {
		float[,] heightMap = terrainData.GetHeights (0, frameNumber, 512, 1);
		for (int i = 0; i < 512; i++) {
			heightMap [0, i] = AudioPeer._audioBand [_band] / 100;
		}
		terrainData.SetHeights (0, frameNumber, heightMap);

		frameNumber++;
	}
}
