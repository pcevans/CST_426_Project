using UnityEngine;

namespace DeckRacer {
	[RequireComponent (typeof(Terrain))]
	public class GenerateFreqSingleBandAddHeightTerrain : MonoBehaviour {
		Terrain terrain;
		TerrainData terrainData;
		public int _band;
		int frameNumber;
		float currHeight;

		void Start () {
			terrain = GetComponent<Terrain> ();
			terrainData = terrain.terrainData;
			frameNumber = 0;
			currHeight = 0.0f;
		}

		void Update () {
			float[,] heightMap = terrainData.GetHeights (0, frameNumber, 512, 1);
			if (AudioPeer._audioBand [_band] > 0.5f)
				currHeight += .001f;
			for (int i = 0; i < 512; i++) {

				heightMap [0, i] = currHeight;
			}
			terrainData.SetHeights (0, frameNumber, heightMap);

			frameNumber++;
		}
	}
}
