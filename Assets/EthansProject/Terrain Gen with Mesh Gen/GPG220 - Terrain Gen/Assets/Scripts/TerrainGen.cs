using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    [System.Serializable]
    public class RegionData
    {
        public string regionName;
        public Color regionColor;
        public float height;
        public bool traverableRegion;

    }

    [RequireComponent(typeof(Terrain))]
    public class TerrainGen : MonoBehaviour
    {
        Terrain terrainComp;
        MeshFilter mfComp;

        public float XScale = 1f;
        public float ZScale = 1f;
        public float NoiseScale = 0.1f;
        public int NumPasses = 4;
        public float noiseZmount;
        public RegionData[] regions;
        public GameObject objPrefab;
        List<GameObject> genedObjs = new List<GameObject>();

        // Use this for initialization
        void Start()
        {
            GenerateTerrain();
        }

        public void GenerateTerrain()
        {
            for (int i = 0; i < genedObjs.Count; i++)
            {
                Destroy(genedObjs[i]);
            }
            genedObjs.Clear();
            // retrieve the terrain
            terrainComp = GetComponent<Terrain>();
            mfComp = GetComponent<MeshFilter>();
            // grab the terrain data

            TerrainData terrainData = terrainComp.terrainData;
            NodeManager.instance.newData = terrainData;

            terrainData.RefreshPrototypes();
            // retrieve the height map
            float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

            for (int x = 0; x < terrainData.heightmapWidth; ++x)
            {
                for (int z = 0; z < terrainData.heightmapHeight; ++z)
                {
                    heightMap[x, z] = noiseZmount;
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

                float newXScale = XScale * 1.5f;
                float newZScale = ZScale * 1.5f;
                float newNoiseScale = NoiseScale * 0.5f;
            }

            terrainData.SetHeights(0, 0, heightMap);
            terrainComp.enabled = false;
            int numVerts = terrainData.heightmapWidth * terrainData.heightmapHeight;
            Vector3[] vertices = new Vector3[numVerts];
            int[] triangles = new int[numVerts * 3 * 2]; // 3 verts per triangle, 2 triangles per quad/square
            Color[] colorMap = new Color[vertices.Length];
            // build up the vertices and triangles
            int vertIndex = 0;
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                for (int z = 0; z < terrainData.heightmapHeight; z++)
                {
                    float height = terrainData.GetHeight(x, z);
                    vertices[vertIndex] = new Vector3(x, height, z);

                    colorMap[vertIndex] = CheckForColorRegion(height);

                    NodeManager.instance.CreateNode(vertices[vertIndex], terrainData, x, z, CheckForTraversableRegion(height));


                    if (height > 10 && height < 15)
                    {


                        float objProbabillity = UnityEngine.Random.Range(0f, 100f);

                        if (objProbabillity <= 5)
                        {
                            GameObject newObj = Instantiate(objPrefab, vertices[vertIndex], Quaternion.identity);
                            genedObjs.Add(newObj);
                        }
                    }

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
            mfComp.mesh.colors = colorMap;

            mfComp.mesh.RecalculateBounds();
            mfComp.mesh.RecalculateNormals();
            NodeManager.instance.Initialize();

        }

        private Color CheckForColorRegion(float currentHeight)
        {
            NodeManager.instance.StatusUpdate("Now checking for region and coloring accordingly");

            for (int i = 0; i < regions.Length; i++)
            {

                if (currentHeight <= regions[i].height)
                {
                    return regions[i].regionColor;
                }
            }

            return Color.red;
        }

        bool CheckForTraversableRegion(float currentHeight)
        {
            for (int i = 0; i < regions.Length; i++)
            {
                if (currentHeight <= regions[i].height)
                    return regions[i].traverableRegion;
            }

            return true;
        }
    }
}