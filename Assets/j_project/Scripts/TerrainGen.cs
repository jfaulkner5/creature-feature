using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public float XScale = 1f;
    public float XOffset = 0f;

    public float ZScale = 1f;
    public float ZOffset = 0f;

    public float BaseHeight = 0.2f;

    public int NumPasses = 1;
    public float PassStrength = 1f;
    public float PassStrengthScale = 0.5f;

    public bool Regenerate = true;

	// Use this for initialization
	void Update ()
    {
        // easy way to make it regenerate without spamming
        if (!Regenerate)
            return;
        Regenerate = false;

        // grab the terrain data
        Terrain terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapWidth;
        int height = terrainData.heightmapHeight;

        // retrieve the heights
        float[,] terrainHeights = terrainData.GetHeights(0, 0, width, height);

        // reset all of the terrain heights
        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < height; ++z)
            {
                terrainHeights[x, z] = BaseHeight;
            }
        }

        // default the current strength to the initial value
        float currentStrength = PassStrength;

        float workingXScale = XScale;
        float workingZScale = ZScale;

        // run all passes
        for (int pass = 0; pass < NumPasses; ++pass)
        {
            // mess with the terrain
            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < height; ++z)
                {
                    float xValue = XOffset + workingXScale * ((float)x / width);
                    float zValue = ZOffset + workingZScale * ((float)z / height);

                    // apply the noise
                    float currentHeight = terrainHeights[x, z];
                    currentHeight += currentStrength * (2f * (Mathf.PerlinNoise(xValue, zValue) - 0.5f));

                    terrainHeights[x, z] = Mathf.Clamp01(currentHeight);
                }
            }

            workingXScale *= 1.25f;
            workingZScale *= 1.5f;
            currentStrength *= PassStrengthScale;
        }

        // set the heights back
        terrainData.SetHeights(0, 0, terrainHeights);
	}
}
