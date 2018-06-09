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

        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        PathingNode[,] grid;

        [HideInInspector]
        public bool debugMode = true;
        float nodeDiameter;
        int gridSizeX, gridSizeY;

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
        public List<PathingNode> path;
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
                    Gizmos.DrawCube(n.node.spacialInfo, Vector3.one * (nodeDiameter - .1f));
                }
            }


        }

    }
}
