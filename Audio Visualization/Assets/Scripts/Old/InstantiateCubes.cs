using UnityEngine;

namespace DeckRacer {
	public class InstantiateCubes : MonoBehaviour {
		public GameObject _sampleCubePrefab;
		readonly GameObject[] _sampleCube = new GameObject[512];
		public float _maxScale;

		// Use this for initialization
		void Start () {
		
			for (int i = 0; i < 512; i++) {
				GameObject _sampleCubeInstance = Instantiate (_sampleCubePrefab);
				_sampleCubeInstance.transform.position = this.transform.position;
				_sampleCubeInstance.transform.parent = this.transform;
				_sampleCubeInstance.name = "SampleCube" + i;
				this.transform.eulerAngles = new Vector3 (0, 0.703175f * i, 0);
				_sampleCubeInstance.transform.position = Vector3.forward * 100;
				_sampleCube [i] = _sampleCubeInstance;
			}
		}
	
		// Update is called once per frame
		void Update () {
			for (int i = 0; i < 512; i++) {
				if (_sampleCube != null) {
					//_sampleCube[i].transform.localScale = new Vector3(10, AudioPeer._samples[i] * _maxScale + 2, 10);
				}
			}
		}
	}
}