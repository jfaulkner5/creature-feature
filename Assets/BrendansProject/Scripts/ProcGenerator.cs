using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace BrendansProject
{
    public class ProcGenerator : MonoBehaviour
    {

        public static ProcGenerator instance;

        public GameObject[] buildings; // Array for building gameobjects

        public GameObject xStreets, zStreets, crossroad, corpse;

        // TODO use from grid
        public int mapWidth = 20;
        public int mapHeight = 20;

        public int buildingFootprint = 3; // Used for spacing out buildings

        public float perlinZoom = 10.0f;

        public int gridOffsetX = 0;
        public int gridOffsetY = 0;

        // TODO change to detect world left point from nodemap
        public float buildingSizeOffset = 1.0f;

        public int minCorpses = 1;
        public int maxCorpses = 5;
        public int minForts = 1;
        public int maxForts = 5;

        int[,] mapGrid;

        private void Awake()
        {
            instance = this;
        }


        public void Generate()
        {

            float seed = Random.Range(0, 100); //random seed for perlin noise

            print("Map seed is " + seed); // Print out what the seed is for the generated map

            mapGrid = new int[mapWidth, mapHeight];

            //generate map data
            for (int h = 0; h < mapHeight; h++)
            {
                for (int w = 0; w < mapHeight; w++)
                {
                    mapGrid[w, h] = (int)(Mathf.PerlinNoise(w / perlinZoom + seed, h / perlinZoom + seed) * 10);
                }

            }

            //build streets
            int x = 0;

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
                            print(rand);
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
                            print(rand);
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
                        Instantiate(corpse, pos, spawnRot, transform);
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
                    //int n = Random.Range(0, buildings.Length);
                    if (result < -2)
                        Instantiate(crossroad, pos, crossroad.transform.rotation, transform);
                    else if (result < -1)
                        Instantiate(xStreets, pos, xStreets.transform.rotation, transform);
                    else if (result < 0)
                        Instantiate(zStreets, pos, zStreets.transform.rotation, transform);
                    else if (result < 1)
                        Instantiate(buildings[0], pos, Quaternion.identity, transform);
                    else if (result < 2)
                        Instantiate(buildings[1], pos, Quaternion.identity, transform);
                    else if (result < 4)
                        Instantiate(buildings[2], pos, Quaternion.identity, transform);
                    else if (result < 6)
                        Instantiate(buildings[3], pos, Quaternion.identity, transform);
                    else if (result < 7)
                        Instantiate(buildings[4], pos, Quaternion.identity, transform);
                    else if (result < 10)
                        Instantiate(buildings[5], pos, Quaternion.identity, transform);
                }
            }



            // Place forts
            int randForts = Random.Range(minForts, maxForts);
            bool fortPlaced = false;
            for (int f = 0; f < randForts; f++)
            {
                fortPlaced = false;
                while (fortPlaced == false)
                {
                int randX = Random.Range(0, mapWidth);
                int randZ = Random.Range(0, mapHeight);
                if (mapGrid[randX, randZ] > 0)
                {
                    ////var gameObjects = Physics.OverlapSphere(transform.position, 1).Except([collider]).Select(function(c) c.gameObject).ToArray();
                    int layerMask = 1 << 9;

                    //print(randX + " , " + randZ);

                    Vector3 destination = new Vector3(randX * 3, 1000, randZ * 3);
                    Vector3 start = new Vector3(randX * buildingFootprint - gridOffsetX + buildingSizeOffset, 50, randZ * buildingFootprint - gridOffsetY + buildingSizeOffset);

                    Ray ray = new Ray(start, -Vector3.up);
                    RaycastHit hit;

                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(ray, out hit,100f,layerMask))
                    //    //if (Physics.Raycast(new Vector3(randX, 100, randZ), -Vector3.up, out hit))
                    {
                        //Debug.DrawLine(ray.origin, hit.point, Color.yellow, Time.deltaTime);
                        //Debug.Log("Hit " + hit.collider.gameObject);
                        Behaviour halo = (Behaviour)hit.collider.gameObject.GetComponent("Halo");
                        halo.enabled = true;
                            // TODO better change of colour
                            Renderer[] renderers = hit.collider.gameObject.GetComponentsInChildren<Renderer>();
                            foreach (Renderer rend in renderers)
                            {
                                rend.material.color = Color.green;
                            }
                            
                        fortPlaced = true;
                    }
                    else
                    {
                        //Debug.DrawLine(ray.origin, hit.point);
                        //Debug.Log("Did not Hit");
                    }


                    }
                }
            }

            // Spawn zombies

        }
    }
}