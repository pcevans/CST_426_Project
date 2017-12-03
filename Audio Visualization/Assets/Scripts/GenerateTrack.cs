using UnityEngine;

public class GenerateTrack : MonoBehaviour {

	void Start () {
		//Vector3[] pointArray = GenerateFreqAvgBandPath.trackPointArray;

		MeshFilter mf = GetComponent<MeshFilter> ();
		var mesh = new Mesh ();
		mf.mesh = mesh;

		var vertices = new Vector3[(50 + 1) * 2];

		for (int i = 0; i <= 50; i++) {
			vertices [2 * i + 0] = new Vector3 (-4, 0, i);
			vertices [2 * i + 1] = new Vector3 (4, 0, i);
		}

		mesh.vertices = vertices;

		var tri = new int[50 * 6];

		for (int i = 0; i < 50; i++) {
			tri [6 * i + 0] = 2 * i + 0;
			tri [6 * i + 1] = 2 * i + 2;
			tri [6 * i + 2] = 2 * i + 1;

			tri [6 * i + 3] = 2 * i + 1;
			tri [6 * i + 4] = 2 * i + 2;
			tri [6 * i + 5] = 2 * i + 3;
		}

		mesh.triangles = tri;

		var normals = new Vector3[102];

		for (int i = 0; i < 102; i++) {
			normals [i] = -Vector3.up;
		}

		mesh.normals = normals; 

		var uv = new Vector2[102];

		for (int i = 0; i < 51; i++) {
			uv [2 * i + 0] = new Vector2 (0, i / 50f);
			uv [2 * i + 1] = new Vector2 (1, i / 50f);
		}

		mesh.uv = uv;
	}
}