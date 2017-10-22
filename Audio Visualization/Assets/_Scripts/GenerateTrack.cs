﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTrack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3[] tpa = GenerateFreqAvgBandPath.trackPointArray;

		float width;
		float height;
		MeshFilter mf = GetComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();
		mf.mesh = mesh;

		Vector3[] vertices = new Vector3[(50 + 1) * 2];

		vertices [0] = new Vector3 (-4, 0, 0);
		vertices [1] = new Vector3 (4, 0, 0);
		for (int i = 1; i <= 50; i++) {
			vertices [i + 0] = new Vector3 (-4, 0, i);
			vertices [i + 2] = new Vector3 (4, 0, i);
		}

		mesh.vertices = vertices;

		int[] tri = new int[50 * 6];


		for (int i = 0; i < 50; i++) {
			
			tri [6 * i + 0] = 4 * i + 0;
			tri [6 * i + 1] = 4 * i + 2;
			tri [6 * i + 2] = 4 * i + 1;

			tri [6 * i + 3] = 4 * i + 2;
			tri [6 * i + 4] = 4 * i + 3;
			tri [6 * i + 5] = 4 * i + 1;
		}

		mesh.triangles = tri;

		Vector3[] normals = new Vector3[4];

		normals [0] = -Vector3.forward;
		normals [1] = -Vector3.forward;
		normals [2] = -Vector3.forward;
		normals [3] = -Vector3.forward;

		mesh.normals = normals; 

		Vector2[] uv = new Vector2[4];

		uv [0] = new Vector2 (0, 0);
		uv [1] = new Vector2 (1, 0);
		uv [2] = new Vector2 (0, 1);
		uv [3] = new Vector2 (1, 1);

		mesh.uv = uv;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}