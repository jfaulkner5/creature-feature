﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace BrendansProject
{
    public class ProcGenerator : MonoBehaviour
    {

        public static ProcGenerator instance;

        [Header("Map Settings")]
        // Sets the total grid size
        public Vector2 mapDimensions;

        // The zoom on the perlin noise (the higher the number the closer / less noise)
        public float perlinZoom = 10.0f;

        [Header("Buildings")]
        public GameObject[] buildings; // Array for building gameobjects

        public int buildingFootprint = 3; // Used for spacing out buildings

        // Size of the buildings set in meters(scale)
        public float buildingSizeOffset = 1.0f;

        public int minForts = 10;
        public int maxForts = 20;

        [Header("Roads")]
        public GameObject xStreets;
        public GameObject zStreets;
        public GameObject crossroad;
           

        [Header("Spawnable Objects")]
        public GameObject zombies; //TODO make array

        public int minZombies = 20;
        public int maxZombies = 50;

        public GameObject corpse;

        public int minCorpses = 10;
        public int maxCorpses = 20;


        private int gridOffsetX = 0;
        private int gridOffsetY = 0;

        private int[,] mapGrid;

        private List<GameObject> corpses;
        public List<Transform> targets;


        public int mapWidth
        {
            get
            {
                return (int)mapDimensions.x;
            }
        }

        public int mapHeight
        {
            get
            {
                return (int)mapDimensions.y;
            }
        }


        private void Awake()
        {
            instance = this;

            // Determine the offset to center the map
            gridOffsetX = (mapWidth * buildingFootprint) / 2;
            gridOffsetY = (mapHeight * buildingFootprint) / 2;

            corpses = new List<GameObject>();
            targets = new List<Transform>();
        }

        /// <summary>
        /// Determines the worldsize using the building footprint and map dimensions
        /// </summary>
        /// <returns></returns>
        public Vector2 GetWorldSize()
        {
            return new Vector2(mapWidth * buildingFootprint, mapHeight * buildingFootprint);
        }

        /// <summary>
        /// Generate city map using perlin noise.
        /// </summary>
        public void Generate()
        {

            float seed = Random.Range(0, 100); //random seed for perlin noise

            print("Map seed is " + seed); // Print out what the seed is for the generated map

            mapGrid = new int[mapWidth, mapHeight];

            //generate map data using perlin noise
            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapHeight; w++)
                {
                    mapGrid[w, h] = (int)(Mathf.PerlinNoise(w / perlinZoom + seed, h / perlinZoom + seed) * 10); // uses the seed to generate different perlin noise
                }

            }

            //build streets
            int x = 0;

            //TODO remove magic numbers
            for (int n = 0; n < 50; n++)
            {
                for (int h = 0; h < mapHeight; h++)
                {
                    mapGrid[x, h] = -1;
                }
                x += Random.Range(3, 3); // Distance between streets
                if (x >= mapWidth) break;
            }

            int z = 0;

            for (int n = 0; n < 10; n++)
            {
                for (int w = 0; w < mapHeight; w++)
                {
                    if (mapGrid[w, z] == -1) // check if there is a road already placed facing the other direction
                        mapGrid[w, z] = -3; //put in a cross round if there is already a road there.
                    else
                        mapGrid[w, z] = -2;
                }
                z += Random.Range(5, 20); // Distance between streets
                if (z >= mapHeight) break;
            }

            // Place corpses

            int randCorpses = Random.Range(minCorpses, maxCorpses);

            bool corpsePlaced = false;
            for (int c = 0; c < randCorpses; c++)
            {
                corpsePlaced = false;
                while (corpsePlaced == false)
                {
                    int randX = Random.Range(0, mapWidth);
                    int randZ = Random.Range(0, mapHeight);
                    if (mapGrid[randX, randZ] == -1 || mapGrid[randX, randZ] == -2 || mapGrid[randX, randZ] == -3)
                    {


                        Quaternion spawnRot = corpse.transform.rotation; // Corpse spawn rotation

                        // Detect if the corpse is spawning on a z or x road and rotate a random direction to suit.

                        if (mapGrid[randX, randZ] == -2)
                        {
                            int rand = Random.Range(0, 2);

                            switch (rand)
                            {
                                case 0:
                                    spawnRot = Quaternion.Euler(new Vector3(-90, 90, 0));
                                    break;
                                case 1:
                                    spawnRot = Quaternion.Euler(new Vector3(-90, -90, 0));
                                    break;
                            }
                        }
                        else if (mapGrid[randX, randZ] == -1)
                        {
                            int rand = Random.Range(0, 2);

                            switch (rand)
                            {
                                case 0:
                                    spawnRot = Quaternion.Euler(new Vector3(-90, 0, 0));
                                    break;
                                case 1:
                                    spawnRot = Quaternion.Euler(new Vector3(-90, 180, 0));
                                    break;
                            }

                        }


                        Vector3 pos = new Vector3(randX * buildingFootprint - gridOffsetX + buildingSizeOffset, 0.2f, randZ * buildingFootprint - gridOffsetY + buildingSizeOffset);
                        GameObject go = Instantiate(corpse, pos, spawnRot, transform);
                        corpses.Add(go); // add to corpse list for later reference
                        corpsePlaced = true;
                    }
                }
            }

            // Instanciate city
            for (int h = 0; h < mapHeight; h++)
            {

                for (int w = 0; w < mapWidth; w++)
                {

                    int result = mapGrid[w, h];
                    //int result = (int)(Mathf.PerlinNoise(w / perlinZoom + seed, h / perlinZoom + seed) * 10); // for values between 1-10. The higher the value it is divided by the closer it is zoomed into the perlin noise

                    Vector3 pos = new Vector3(w * buildingFootprint - gridOffsetX + buildingSizeOffset, 0, h * buildingFootprint - gridOffsetY + buildingSizeOffset);


                    //TODO make better, spawn using the a range for each building type, create a class for buildings
                    switch (result)
                    {
                        case -3:
                            Instantiate(crossroad, pos, crossroad.transform.rotation, transform);
                            break;
                        case -2:
                            Instantiate(xStreets, pos, xStreets.transform.rotation, transform);
                            break;
                        case -1:
                            Instantiate(zStreets, pos, zStreets.transform.rotation, transform);
                            break;
                        case 0:
                            Instantiate(buildings[0], pos, Quaternion.identity, transform);
                            break;
                        case 1:
                            Instantiate(buildings[1], pos, Quaternion.identity, transform);
                            break;
                        case 2:
                        case 3:
                            Instantiate(buildings[2], pos, Quaternion.identity, transform);
                            break;
                        case 4:
                        case 5:
                            Instantiate(buildings[3], pos, Quaternion.identity, transform);
                            break;
                        case 6:
                            Instantiate(buildings[4], pos, Quaternion.identity, transform);
                            break;
                        case 7:
                        case 8:
                        case 9:
                            // Randomly choose orientation
                            Quaternion randomRotation = Quaternion.Euler(0, 90 * Random.Range(0, 5), 0);
                            Instantiate(buildings[5], pos, randomRotation, transform);
                            break;
                    }

                }
            }


            //TODO Spread forts across map
            // Create during spawning. use a ratio that checks a spawn chance agaisnt how many need to be spawned.
            // possible performance increase and also will spread out better as not completely random.

            // Place forts
            int randForts = Random.Range(minForts, maxForts);
            bool fortPlaced = false;
            for (int f = 0; f < randForts; f++)
            {
                fortPlaced = false;
                while (fortPlaced == false)
                {
                    //Generate a random location on the map
                    int randX = Random.Range(0, mapWidth);
                    int randZ = Random.Range(0, mapHeight);
                    if (mapGrid[randX, randZ] > 0)
                    {
                        int layerMask = 1 << 9; // Set layermask to unwalkable

                        //print(randX + " , " + randZ);

                        // Raycast to see if there is a building
                        Vector3 start = new Vector3(randX * buildingFootprint - gridOffsetX + buildingSizeOffset, 50, randZ * buildingFootprint - gridOffsetY + buildingSizeOffset);

                        Ray ray = new Ray(start, -Vector3.up);
                        RaycastHit hit;

                        // Does the ray intersect any objects on the unwalkable layer
                        if (Physics.Raycast(ray, out hit, 100f, layerMask))

                        {
                            //Debug.Log("Hit " + hit.collider.gameObject);
                            // Create a behaviour for the halo and enable it
                            Behaviour halo = (Behaviour)hit.collider.gameObject.GetComponent("Halo");
                            halo.enabled = true;

                            // change all the material colors
                            Renderer[] renderers = hit.collider.gameObject.GetComponentsInChildren<Renderer>();
                            foreach (Renderer rend in renderers)
                            {
                                rend.material.color = Color.cyan;
                            }

                            // Adds to the targets list which zombies will attack
                            targets.Add(hit.collider.gameObject.transform);

                            fortPlaced = true;
                        }
                        else
                        {
                            //Debug.Log("Did not Hit");
                        }


                    }
                }
            }

            // Spawn zombies
            
            int randZombies = Random.Range(minZombies, maxZombies); // how many to spawn

            for (int e = 0; e < randZombies; e++)
            {

                int randZ = Random.Range(0, corpses.Count); // get a random corpse to spawn at

                Instantiate(zombies, corpses[randZ].transform.position, Quaternion.identity, transform);
                
            }

            }
    }
}