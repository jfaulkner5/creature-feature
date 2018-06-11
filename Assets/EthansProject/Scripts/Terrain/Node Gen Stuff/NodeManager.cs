using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class NodeManager : MonoBehaviour
    {
        int gridSizeX, gridSizeY;
        public List<PathingNode> nodes = new List<PathingNode>();
        Vector2 gridWorldSize;
        #region Single Instance
        public static NodeManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Multip of, " + this + " existing in scene!");
                DestroyImmediate(this);

            }
            else
            {
                instance = this;
            }
        }
        #endregion
        //TODO Stop being trash
        public LayerMask unwalkableMask;
        //public Vector2 gridWorldSize;
        [Range(0.1f, 0.8f)]
        public float nodeRadius;
        PathingNode[,] grid;
        TerrainData newData;
        [HideInInspector]
        public bool debugMode = true;
        float nodeDiameter;
        //TODO Stop being trash
        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
           
            nodeDiameter = nodeRadius * 2;
            gridWorldSize = new Vector2(newData.heightmapWidth, newData.heightmapHeight);
            //gridSizeX = Mathf.RoundToInt(newData.heightmapWidth / nodeDiameter);
            //gridSizeY = Mathf.RoundToInt(newData.heightmapHeight / nodeDiameter);
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            //CreateGrid();
        }

        private void Update()
        {
            //Dont do this lmao. yikes CreateGrid();
        }
        /// <summary>
        /// Creates the nodes in the space of the grid size.
        /// </summary>
        public void CreateGrid(Vector3 vertPoint,TerrainData data,  int x, int z)
        {
            newData = data;
            grid = new PathingNode[data.heightmapWidth, data.heightmapHeight];
            //Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            

            // Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
            bool walkable = !(Physics.CheckSphere(vertPoint, nodeRadius, unwalkableMask));
            grid[x, z] = new PathingNode(walkable, vertPoint, x, z);
            nodes.Add(grid[x, z]);
        }

        /// <summary>
        /// Gets all the neigbours of the passing node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<PathingNode> GetNeigbours(PathingNode node)
        {
            List<PathingNode> neigbours = new List<PathingNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neigbours.Add(grid[checkX, checkY]);
                    }
                }
            }
            return neigbours;
        }
        //TODO Stop being trash
        /// <summary>
        /// Recives the node from world pace position passed through
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public PathingNode NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + newData.heightmapWidth / 2) / newData.heightmapWidth;
            float percentY = (worldPosition.z + newData.heightmapHeight / 2) / newData.heightmapHeight;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            Debug.Log("Found: " + grid[x, y] + ". At: " + worldPosition);

            return grid[x, y];

        }

         /// <summary>
         /// Draws the nodews and determains whether to make them different colors. 
         /// </summary>
        void OnDrawGizmos()
        {
            if (!debugMode)
                return;

           // Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid != null)
            {
                foreach (PathingNode n in nodes)
                {
                    Gizmos.color = (n.traverable) ? Color.white : Color.red;
                    //if (path != null && path.Contains(n))
                    //    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(n.node.spacialInfo, nodeDiameter /2);
                }
            }


        }

    }
}
