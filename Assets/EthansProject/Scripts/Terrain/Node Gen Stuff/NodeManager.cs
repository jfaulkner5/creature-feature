using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class NodeManager : MonoBehaviour
    {

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
        public Vector2 gridWorldSize;
        [Range(0.1f, 0.8f)]
        public float nodeRadius;
        PathingNode[,] grid;

        [HideInInspector]
        public bool debugMode = true;
        float nodeDiameter;
        int gridSizeX, gridSizeY;
        public List<PathingNode> path;
        //TODO Stop being trash
        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            CreateGrid();
        }

        private void Update()
        {
            //Dont do this lmao. yikes CreateGrid();
        }
        /// <summary>
        /// Creates the nodes in the space of the grid size.
        /// </summary>
        public void CreateGrid()
        {
            grid = new PathingNode[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    grid[x, y] = new PathingNode(walkable, worldPoint, x, y);
                }
            }
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
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];

        }

         /// <summary>
         /// Draws the nodews and determains whether to make them different colors. 
         /// </summary>
        void OnDrawGizmos()
        {
            if (!debugMode)
                return;

            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid != null)
            {
                foreach (PathingNode n in grid)
                {
                    Gizmos.color = (n.traverable) ? Color.white : Color.red;
                    if (path != null && path.Contains(n))
                        Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(n.node.spacialInfo, 1 * (nodeDiameter - .1f));
                }
            }


        }

    }
}
