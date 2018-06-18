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
        int xWidth { get { return (WorldSize > 0) ? WorldSize * 16 : 16; } }
        int zWidth { get { return (WorldSize > 0) ? WorldSize * 16 : 16; } }

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
            texture = new Texture2D(16, 16);
            texture = (Texture2D)mapList_WorldList.list[Random.Range(0, mapList_WorldList.list.Count - 1)];
            pixels = texture.GetPixels();

            CreateGridRandom();

            PathGrid.CreateGrid(mask, 16, 16, .5f, transform);
    }

        void CreateGridRandom()
        {
            grid = new GameObject[xWidth, zWidth];
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
            return array[x * 16 + z];
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
