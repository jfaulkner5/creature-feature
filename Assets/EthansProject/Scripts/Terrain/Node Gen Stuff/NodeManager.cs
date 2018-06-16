using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class NodeManager : MonoBehaviour
    {
        int gridSizeX, gridSizeZ;
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

        public LayerMask unwalkableMask;
        //public Vector2 gridWorldSize;
        [Range(0.1f, 0.8f)]
        public float nodeRadius;
        PathingNode[,] grid;
        [HideInInspector]
        public TerrainData newData;
        [HideInInspector]
        public bool debugMode = true;
        float nodeDiameter;

        public int MaxSize
        {
            get { return gridSizeX * gridSizeZ; }
        }

        public bool _initialized = false;

        public void Initialize()
        {
            _initialized = true;

            nodeDiameter = nodeRadius * 2;

            gridSizeX = Mathf.RoundToInt(newData.heightmapWidth);
            gridSizeZ = Mathf.RoundToInt(newData.heightmapHeight);

        }
        public void StatusUpdate(string newStatus)
        {
            if (status == newStatus)
                return;

            status = newStatus;

            if (!debugMode)
                return;

            Debug.LogWarning(status);

        }

        private void Start()
        {
            debugMode = false;
        }

        private void Update()
        {
            //Dont do this lmao. yikes CreateGrid();
        }

        /// <summary>
        /// Creates the nodes in the space of the grid size.
        /// </summary>
        public void CreateNode(Vector3 vertPoint, TerrainData data, int x, int z, bool accessableRegion)
        {
            newData = data;

            if (nodes.Count == 0)
                grid = new PathingNode[newData.heightmapWidth, newData.heightmapHeight];

            Vector3 nodeOffset = new Vector3(0, nodeRadius, 0);
            Vector3 newNodePoint = vertPoint + nodeOffset;

            bool walkable = !(Physics.CheckSphere(newNodePoint, nodeRadius, unwalkableMask));

            if (!accessableRegion)
                walkable = false;

            grid[x, z] = new PathingNode(walkable, newNodePoint, x, z);

            nodes.Add(grid[x, z]);

            // Debug.Log(grid[x, z].gridX + ", " + grid[x, z].gridY );
        }

        /// <summary>
        /// Gets all the neigbours of the passing node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string status;
        public List<PathingNode> GetNeigbours(PathingNode node)
        {
            StatusUpdate("Now trying to get a list of neigbours");

            List<PathingNode> neigbours = new List<PathingNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeZ)
                        neigbours.Add(grid[checkX, checkY]);
                }
            }
            return neigbours;
        }

        /// <summary>
        /// Recives the node from world pace position passed through
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public PathingNode NodeFromWorldPoint(Vector3 worldPosition)
        {

            Vector3 posOffset = worldPosition - newData.bounds.min;

            int x = Mathf.RoundToInt(posOffset.x);
            int y = Mathf.RoundToInt(posOffset.z);

            // Debug.Log("Found: " + grid[x, y].gridX + ", " + grid[x, y].gridY + ". At: " + worldPosition + " : " + x + ","+y);

            return grid[x, y];

        }

        public List<PathingNode> path = new List<PathingNode>();

        /// <summary>
        /// Draws the nodews and determains whether to make them different colors. 
        /// </summary>
        void OnDrawGizmos()
        {
            if (!debugMode)
                return;

            if (grid != null)
            {

                foreach (PathingNode n in grid)
                {
                    if (n.traverable)
                        Gizmos.color = Color.white;
                    else
                        continue;

                    if (path != null && path.Contains(n))
                        Gizmos.color = Color.blue;

                    Gizmos.DrawCube(n.node.spacialInfo, Vector3.one *  nodeDiameter / 2);
                }
            }
        }

    }
}
