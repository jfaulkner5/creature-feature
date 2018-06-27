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
        public float ResourceSpawn = .1f;
        public Vector2 seedOffset;
        public float perlinMod = 2;
        [Tooltip("0 is ground")]
        public Material wallMat;
        [Tooltip("The Grid Object")]
        public bool showNavLocations;
        public GameObject gridObject;
        public LayerMask mask;
        
        [Header("Data")]
        public MapList mapList_HomeList;
        public MapList mapList_WorldList;
        public ResourceList resources;

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
            if (PathGrid.grid != null && showNavLocations)
            {
                for (int x = 0; x < xWidth; x++)
                {
                    for (int z = 0; z < zWidth; z++)
                    {
                        if (PathGrid.grid[x, z].BackPath)
                        {
                            Gizmos.color = Color.blue;
                        }
                        else if (PathGrid.grid[x, z].walkable)
                        {
                            Gizmos.color = Color.green;
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                        }
                        Gizmos.DrawSphere(PathGrid.grid[x, z].worldPosition, .1f);
                    }
                }
            }            
        }

        // Use this for initialization
        void Start() {
            Setup();
        }

        void Setup()
        {
            
            #region createLevelImage
            // This creates a fliped image, but. oh well...

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

            #endregion

            pixels = texture.GetPixels();

            grid = new GameObject[xWidth, zWidth];

            CreateGridFromHome();

            PathGrid.CreateGrid(mask, gridSize, gridSize, .5f, transform);
        }        

        void CreateGridFromHome()
        {
            grid = new GameObject[xWidth, zWidth];
            for (int x = 0; x < xWidth; x++)
            {
                for (int z = 0; z < zWidth; z++)
                {
                    GameObject newGridObject = Instantiate<GameObject>(gridObject, this.transform);
                    newGridObject.transform.position = new Vector3(x, -0.5f + GetVerticalFromPixelWithNoise(x,z,pixels, WallBias), z);
                    if (newGridObject.transform.position.y > -.5f)
                    {
                        newGridObject.layer = 9;
                    }
                    grid[x, z] = newGridObject;
                    
                    //populate grid with Resource
                    if (x > 16 && z > 16 && GetPerlin(x,z) < ResourceSpawn)
                    {
                        Debug.Log(GetPerlin(x, z) + " x" + x + " z" + z);

                        if (newGridObject.transform.position.y < 0)
                        {
                            GameObject newResource = Instantiate<GameObject>(resources.arrayResourceTypes[Random.Range(0, resources.arrayResourceTypes.Length - 1)]);
                            newResource.transform.position = new Vector3(x, 0, z);
                        }                        
                    }
                }
            }
        }
        /// <summary>
        /// Converts 2d position into the 1d position of the color array. using gridSize as its scaler.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        Color GetPixelFromPosition(int x, int z, Color[] array)
        {
            return array[(x * gridSize) + z];
        }

        /// <summary>
        /// uses gets the gray colour and then applys noise.
        /// Will return a bias 0 or 1. as a result of perlin noise, if the roll is less than the bias it will return a 0.
        /// </summary>
        /// <param name="x">position</param>
        /// <param name="z">position</param>
        /// <param name="pixels">the Image array</param>
        /// <param name="bias">Use 0-1 values</param>
        /// <returns> 0-1 position</returns>
        float GetVerticalFromPixelWithNoise(int x, int z, Color[] pixels, float bias)
        {
            float color = GetVerticalFromColor(GetPixelFromPosition(x, z, pixels));
            float Y = GetPerlin(x, z);
            //Debug.Log("posX: " + x +" posZ: "+ z + " return: " + Mathf.Clamp((color - Y),0,1));            
            return(Mathf.Clamp((color - Y), 0, 1) < bias) ? 0 : 1;          
            
        }

        #region GetPerlin

        /// <summary>
        /// Perlin with custom position, in class world size, inclass seedOffset
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        float GetPerlin(int x, int y)
        {
            return GetPerlin(xWidth, zWidth, x, y,seedOffset.x,seedOffset.y);
        }

        /// <summary>
        /// Perlin with Custom world size, position and offset;
        /// </summary>
        /// <param name="xwidth"></param>
        /// <param name="ywidth"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        float GetPerlin(int xwidth,int ywidth,int x, int y, float seedOffsetX, float seedOffsetY)
        {
            return (Mathf.PerlinNoise((x + seedOffsetX) / xwidth, (y + seedOffsetY) / ywidth));
        }

        #endregion

        /// <summary>
        /// Returns a 0-1 based on the grayscale colour.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        float GetVerticalFromColor(Color color)
        {
            return color.grayscale;
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
