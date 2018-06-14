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

        public GameObject objToSpawnForRegion;
        public float noiseCap = 0.4f;
        public float spawnPossiblity = 3;
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
        public float baseHeight;
        public RegionData[] regions;
        public GameObject[] objPrefabs;
        List<GameObject> genedObjs = new List<GameObject>();
        public float passStrengthScale;
        public float passStrength;
        public float passNoiseScalse;

        public float min = 0.9f;
        public float max = 1.1f;


        // Use this for initialization
        void Start()
        {
            GenerateTerrain();
        }

        public void GenerateTerrain()
        {
            // Runs through the gernerated objects and deletes them and clears the list to that objects like trees are still there when world is regened
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

            // retrieve the height map
            float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

            //Resets the height map
            for (int x = 0; x < terrainData.heightmapWidth; ++x)
            {
                for (int z = 0; z < terrainData.heightmapHeight; ++z)
                {
                    heightMap[x, z] = baseHeight;
                }
            }
            float currentStrength = passStrength;
            float newxScale = XScale;
            float newzScale = ZScale;
            float newNoiseScale = NoiseScale;
            // run each individual pass
            for (int pass = 0; pass < NumPasses; ++pass)
            {
                //Generate noise
                for (int x = 0; x < terrainData.heightmapWidth; ++x)
                {
                    for (int z = 0; z < terrainData.heightmapHeight; ++z)
                    {
                        heightMap[x, z] += currentStrength * 2f * newNoiseScale * (Mathf.PerlinNoise(newxScale * x / terrainData.heightmapWidth,
                                                                                newzScale * z / terrainData.heightmapHeight) - 0.5f) * UnityEngine.Random.Range(min, max);

                    }
                }
                // 
                currentStrength *= passStrengthScale;
                newxScale *= passStrengthScale;
                newzScale *= passStrengthScale;
                newNoiseScale *= passNoiseScalse;
            }

            terrainData.SetHeights(0, 0, heightMap);
            terrainComp.enabled = false;
            int numVerts = terrainData.heightmapWidth * terrainData.heightmapHeight;
            Vector3[] vertices = new Vector3[numVerts];
            int[] triangles = new int[numVerts * 3 * 2]; // 3 verts per triangle, 2 triangles per quad/square
            Color[] colorMap = new Color[vertices.Length];
            // build up the vertices and triangles
            int vertIndex = 0;

            //Generate the mesh vertices and objects
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                for (int z = 0; z < terrainData.heightmapHeight; z++)
                {
                    float height = terrainData.GetHeight(x, z);
                    vertices[vertIndex] = new Vector3(x, height, z);
                    //Check the vert for its colour
                    colorMap[vertIndex] = CheckForColorRegion(height);
                    // place a node.
                    NodeManager.instance.CreateNode(vertices[vertIndex], terrainData, x, z, CheckForTraversableRegion(height));

                    //if inbetween height spawn a object
                    //Hack: make better object gen system.

                    SpawnObject(vertices[vertIndex], x, z, height);

                    //Builds the vertices
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

        void SpawnObject(Vector3 vertPoint, int x, int z, float currentHeight)
        {

            for (int i = 0; i < regions.Length; i++)
            {
                if (currentHeight <= regions[i].height)
                {
                    if (!regions[i].objToSpawnForRegion)
                        return;

                    float noise = Mathf.PerlinNoise(x / (float)terrainComp.terrainData.heightmapWidth, z / (float)terrainComp.terrainData.heightmapHeight);
                    //print(noise);
                    if (noise <= regions[i].noiseCap)
                    {

                        int objToSpawn = UnityEngine.Random.Range(0, objPrefabs.Length);
                        float objProbabillity = UnityEngine.Random.Range(0f, 100f);

                        if (objProbabillity <= regions[i].spawnPossiblity)
                        {
                            GameObject newObj = Instantiate(regions[i].objToSpawnForRegion, vertPoint, Quaternion.identity);
                            genedObjs.Add(newObj);
                        }

                    }
                }
            }


        }

        /// <summary>
        /// checks the passed through height for its assigned colour in the regions
        /// </summary>
        /// <param name="currentHeight"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks of the current hieght is a traversable region
        /// </summary>
        /// <param name="currentHeight"></param>
        /// <returns></returns>
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