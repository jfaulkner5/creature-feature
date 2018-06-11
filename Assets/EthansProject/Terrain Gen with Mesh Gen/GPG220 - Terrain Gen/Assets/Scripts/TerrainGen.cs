using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
	[RequireComponent(typeof(Terrain))]
	public class TerrainGen : MonoBehaviour
	{
		Terrain terrainComp;
		MeshFilter mfComp;

		public float XScale = 1f;
		public float ZScale = 1f;
		public float NoiseScale = 0.1f;
		public int NumPasses = 4;

		// Use this for initialization
		void Start()
		{
			// retrieve the terrain
			terrainComp = GetComponent<Terrain>();
			mfComp = GetComponent<MeshFilter>();

			// grab the terrain data
			TerrainData terrainData = terrainComp.terrainData;
            
            // retrieve the height map
            float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

			for (int x = 0; x < terrainData.heightmapWidth; ++x)
			{
				for (int z = 0; z < terrainData.heightmapHeight; ++z)
				{
					heightMap[x, z] = 0.05f;
				}
			}

			// run each individual pass
			for (int pass = 0; pass < NumPasses; ++pass)
			{
				for (int x = 0; x < terrainData.heightmapWidth; ++x)
				{
					for (int z = 0; z < terrainData.heightmapHeight; ++z)
					{
						heightMap[x, z] += 2f * NoiseScale * (Mathf.PerlinNoise(XScale * x / terrainData.heightmapWidth,
																				ZScale * z / terrainData.heightmapHeight) - 0.5f);
					}
				}

				XScale *= 1.5f;
				ZScale *= 1.5f;
				NoiseScale *= 0.5f;
			}

			terrainData.SetHeights(0, 0, heightMap);
            terrainComp.enabled = false;
			int numVerts = terrainData.heightmapWidth * terrainData.heightmapHeight;
			Vector3[] vertices = new Vector3[numVerts];
			int[] triangles = new int[numVerts * 3 * 2]; // 3 verts per triangle, 2 triangles per quad/square

			// build up the vertices and triangles
			int vertIndex = 0;
			for (int x = 0; x < terrainData.heightmapWidth; ++x)
			{
				for (int z = 0; z < terrainData.heightmapHeight; ++z)
				{
					float height = terrainData.GetHeight(x, z);
					vertices[vertIndex] = new Vector3(x, height, z);
                    NodeManager.instance.CreateGrid(vertices[vertIndex], terrainData,  x, z);
					if ((x < (terrainData.heightmapWidth - 1)) && (z < (terrainData.heightmapHeight - 1)))
					{
						triangles[(vertIndex * 6) + 0] = vertIndex + 1;
						triangles[(vertIndex * 6) + 1] = vertIndex + terrainData.heightmapWidth + 1;
						triangles[(vertIndex * 6) + 2] = vertIndex + terrainData.heightmapWidth;
						triangles[(vertIndex * 6) + 3] = vertIndex + terrainData.heightmapWidth;
						triangles[(vertIndex * 6) + 4] = vertIndex;
						triangles[(vertIndex * 6) + 5] = vertIndex + 1;
					}

					++vertIndex;
				}
			}

			mfComp.mesh.vertices = vertices;
			mfComp.mesh.triangles = triangles;
			mfComp.mesh.RecalculateBounds();
			mfComp.mesh.RecalculateNormals();
		}
	}
}