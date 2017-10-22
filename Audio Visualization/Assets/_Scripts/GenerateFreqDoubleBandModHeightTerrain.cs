using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Terrain))]
public class GenerateFreqDoubleBandModHeightTerrain : MonoBehaviour {
	Terrain terrain;
	TerrainData terrainData;
	public int _increaseBand, _decreaseBand;
	int frameNumber;
	float currHeight;

	void Start () {
		terrain = GetComponent<Terrain> ();
		terrainData = terrain.terrainData;
		frameNumber = 0;
		currHeight = .05f;
	}

	void Update () {
		float[,] heightMap = terrainData.GetHeights (0, frameNumber, 512, 1);
		if (AudioPeer._audioBand [_increaseBand] > 0.5f)
			currHeight += .001f * AudioPeer._audioBand [_increaseBand];
		if (AudioPeer._audioBand [_decreaseBand] > 0.5f)
			currHeight -= .001f * AudioPeer._audioBand [_decreaseBand];
		for (int i = 0; i < 512; i++) {

			heightMap [0, i] = currHeight;
		}
		terrainData.SetHeights (0, frameNumber, heightMap);

		frameNumber++;
	}
}
