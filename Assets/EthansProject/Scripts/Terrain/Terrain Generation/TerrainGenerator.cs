using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
     
    // Use this for initialization
    void Start()
    {
        GenerateMap();


    }
    public float xScale = 1f;
    public float xOffset = 0f;

    public float baseHeight = .2f;

    public float zScale = 1f;
    public float zOffset = 0f;

    public int numPasses = 1;
    public float PassStrength = 1;
    public float passStrengthScale = 0.3f;
    public bool regenerate = true;

    public float treeSpawnPossiblitity = .3f;
    public GameObject treePrefab;
    // Update is called once per frame
    private void Update()
    {
         
        if (!regenerate)
            return;

        regenerate = false;

        //grab some data
        Terrain terr = GetComponent<Terrain>();
        TerrainData terrData = terr.terrainData;
        int width = terrData.heightmapWidth;
        int height = terrData.heightmapHeight;

        //recive the height
        float[,] terrHieght = terrData.GetHeights(0, 0, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                terrHieght[x, z] = baseHeight;
            }
        }

        float currentStrength = PassStrength;

        float newxScale = xScale;
        float newzScale = zScale;
        for (int pass = 0; pass < numPasses; pass++)
        {
            //messing with the terrain
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    float xValue = xOffset + newxScale * ((float)x / width);
                    float zValue = zOffset + newzScale * ((float)z / height);

                    float currentHeight = terrHieght[x, z];
                    currentHeight += currentStrength * 2f * (Mathf.PerlinNoise(xValue, zValue) - 0.5f);

                    terrHieght[x, z] = Mathf.Clamp01(currentHeight);

                    float treeChance = Random.Range(0, 100);
                    if (currentHeight <= 0.05f && currentHeight >= -0.05f)
                    {
                        if (treeChance >= treeSpawnPossiblitity)
                        {
                            Vector3 treePos = new Vector3((x/xScale), 1, (z/zScale));
                            Instantiate(treePrefab, treePos, treePrefab.transform.rotation);
                        }
                    }
                }
            }

            newxScale *= passStrengthScale;
            newzScale *= passStrengthScale;
            currentStrength *= passStrengthScale;
        }



        //set the heights back
        terrData.SetHeights(0, 0, terrHieght);
    }

    public void GenerateMap()
    {
         
    }

}
