using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public class WorldBuilder : MonoBehaviour {

        [Header("WorldSettings")]
        [Tooltip("World size is mesures in 16x16 chunks. This value is currantly min clamped at 1")]
        public int WorldSize;
        [Range(0.1f,1)]
        public float WallBias = 0.8f;
        public Vector2 seedOffset;
        public float perlinMod = 2;
        [Tooltip("The Grid Object")]
        public GameObject gridObject;
        public LayerMask mask;
        
        [Header("MapLists")]
        public MapList mapList_HomeList;
        public MapList mapList_WorldList;

        //
        int gridSize { get { return (WorldSize > 0) ? WorldSize * 16 : 16; } }
        int xWidth { get { return gridSize; } }
        int zWidth { get { return gridSize; } }

        Color[] pixels;

        GameObject[,] grid;

        Texture2D texture;

        private void OnValidate()
        {
        }

        private void OnDrawGizmos()
        {
            if (PathGrid.grid != null)
            {
                for (int x = 0; x < xWidth; x++)
                {
                    for (int z = 0; z < zWidth; z++)
                    {
                        Gizmos.color = (PathGrid.grid[x, z].walkable) ? Color.green:Color.red;
                        Gizmos.DrawSphere(PathGrid.grid[x, z].worldPosition, .1f);
                    }
                }
            }            
        }

        // Use this for initialization
        void Start() {
            
            Setup();
            CreateGridRandom();

            PathGrid.CreateGrid(mask, gridSize, gridSize, .5f, transform);
        }

        void Setup()
        {
            texture = new Texture2D(gridSize, gridSize);
            Texture2D image;

            int imageCount = WorldSize * WorldSize;
            
            for (int xIndex = 0; xIndex < WorldSize; xIndex++)
            {
                for (int zIndex = 0; zIndex < WorldSize; zIndex++)
                {
                    MapList map = (xIndex == 0 && zIndex == 0) ? mapList_HomeList : mapList_WorldList;
                    image = (Texture2D)map.list[Random.Range(0, map.list.Count - 1)];

                    texture.SetPixels(xIndex * 16, zIndex * 16, image.width, image.height, image.GetPixels(), 0);
                }
            }

            /*
             * WHAT WAS IS DOING HERE??
             * 
             * This ^^ is getting images and applying them to parts of the larger texture that will need to be flipped before being fed into the grid.
             */
            
            pixels = texture.GetPixels();

            grid = new GameObject[xWidth, zWidth];

        }

        void CreateGridRandom()
        {            
            for (int x = 0; x < xWidth; x++)
            {
                for (int z = 0; z < zWidth; z++)
                {
                    GameObject newGridObject = Instantiate<GameObject>(gridObject, this.transform);
                    newGridObject.transform.position = new Vector3(x, VerticalPos(WallBias,x,z) - 0.5f, z);
                    if (newGridObject.transform.position.y > -.5f)
                    {
                        newGridObject.layer = 9;
                    }
                    grid[x, z] = newGridObject;
                }
            }
        }

        void CreateGridFromHome()
        {
            grid = new GameObject[xWidth, zWidth];
            for (int x = 0; x < xWidth; x++)
            {
                for (int z = 0; z < zWidth; z++)
                {
                    GameObject newGridObject = Instantiate<GameObject>(gridObject, this.transform);
                    newGridObject.transform.position = new Vector3(x, -0.5f + GetVerticalFromColor(GetPixelFromPosition(x, z, pixels)), z);
                    if (newGridObject.transform.position.y > -.5f)
                    {
                        newGridObject.layer = 9;
                    }
                    grid[x, z] = newGridObject;
                }
            }
        }

        float GetVerticalFromColor(Color color)
        {
            return color.grayscale;
        }

        Color GetPixelFromPosition(int x, int z, Color[] array)
        {
            return array[x * gridSize + z];
        }

        #region VerticalPos

        /// <summary>
        /// Returns a random 0 or 1;
        /// </summary>
        /// <returns></returns>
        float VerticalPos()
        {
            return (Random.Range(0, 100) < 50) ? 0 : 1;
        }
        /// <summary>
        /// Will return a bias random 0 or 1. Where the roll is less than the bias it will return a 0.
        /// </summary>
        /// <param name="bias"> Use 0-1 values</param>
        /// <returns></returns>
        float VerticalPos(float bias)
        {
            bias *= 100;
            return (Random.Range(0, 100) < bias) ? 0 : 1;
        }
        /// <summary>
        /// Will return a bias 0 or 1. as a result of perlin noise, if the roll is less than the bias it will return a 0;
        /// </summary>
        /// <param name="bias">Use 0-1 values</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        float VerticalPos(float bias,float x, float y)
        {
            float Y = (Mathf.PerlinNoise((x + seedOffset.x) / xWidth, (y + seedOffset.y) / zWidth));
            return (Y < bias) ? 0 : 1;
        }
        #endregion
    }
}
