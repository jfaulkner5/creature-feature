using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BrendansProject
{
    /// <summary>
    /// Stores the information for the grid of nodes in the world and handles the grid creation and weights blurring.
    /// </summary>
    public class NodeGrid : MonoBehaviour
    {

        public bool displayGridGizmos; // Toggle grid gizmos on/off

        public float nodeRadius; // How much space each individual node covers
        private float nodeDiameter; // Used to Determine how many nodes can fit in the grid

        public int obstacleProximityPenalty = 10;

        Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

        public LayerMask unwalkableMask; // Layer that is not traversable
        private LayerMask walkableMask = 0;

        public TerrainType[] walkableRegions;


        public Node[,] nodeGrid; // 2 Dimentional array used to represent the node grid

        private Vector2 gridWorldSize; // Defines the area in worldsize that the grid will cover

        int gridSizeX, gridSizeY;

        int penaltyMin = int.MaxValue;
        int penaltyMax = int.MinValue;

        int blurPenaltySize = 3;

        private void Start()
        {
            nodeDiameter = nodeRadius * 2;

            gridWorldSize = ProcGenerator.instance.GetWorldSize();

            // Convert the world size(mtrs) into node amounts using the nodes diameter
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Round value so there are no partial nodes
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);



            // Add all walkable regions to the Regions dictoinary
            foreach (TerrainType region in walkableRegions)
            {
                walkableMask.value |= region.terrainMask.value;
                walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty); // Add terrainMask value to dictionary using Mathf.log
            }

            ProcGenerator.instance.Generate(); // Generate city map first
            CreateGrid();
            GetFortNodes();
        }

        /// <summary>
        /// Maximum size of grid.
        /// </summary>
        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }

        /// <summary>
        /// Find each point a walkable node is going to occupy in world space. 
        /// </summary>
        private void CreateGrid()
        {
            nodeGrid = new Node[gridSizeX, gridSizeY]; // Create array with the specified size.

            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; // Find the bottom left point of our grid in world space

            // Check is a node is walkable then add it to the grid array
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                    int movementPenalty = 0;


                    Ray ray = new Ray(worldPoint + Vector3.up * 20, Vector3.down); // 50 can be moved down as the terrain is flat
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 20, walkableMask))
                    {  // 100 can be moved down as the terrain is flat
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }


                    nodeGrid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(blurPenaltySize);

        }

        /// <summary>
        /// Smooth weights using blurring.
        /// Using a box blur. https://en.wikipedia.org/wiki/Box_blur
        /// </summary>
        /// <param name="blurSize"></param>
        void BlurPenaltyMap(int blurSize)
        {

            int kernelSize = blurSize * 2 + 1; // kernal must be an odd number so there is a central square
            int kernelExtents = (kernelSize - 1) / 2; // How many squares between the central square and the kernal edge

            // Temporary grids to store the horizontal and vertical passes in
            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            // Loop through the kernals 
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += nodeGrid[sampleX, y].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - nodeGrid[removeIndex, y].movementPenalty + nodeGrid[addIndex, y].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                // Determine & set blur amount using the kernal size
                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                nodeGrid[x, 0].movementPenalty = blurredPenalty;

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    nodeGrid[x, y].movementPenalty = blurredPenalty;

                    // Ensure penalty has does not go over the max value
                    if (blurredPenalty > penaltyMax)
                    {
                        penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < penaltyMin)
                    {
                        penaltyMin = blurredPenalty;
                    }
                }
            }

        }

        /// <summary>
        /// Generates a list of nodes that a currently neighboured to the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            // Loop 
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue; // Skip if the node is the specified node

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    // Check if the node is inside of the grid then add to the neighbours list
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(nodeGrid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Get the node at the front of the fort.
        /// </summary>
        public void GetFortNodes()
        {

            foreach (GameObject fort in ProcGenerator.instance.forts)
            {

                // Set target pos to closest walkable nodes world pos
                fort.GetComponent<Unit>().targetPos = GetClosestWalkable(NodeFromWorldPoint(fort.transform.position)).worldPosition;

                //ProcGenerator.instance.targets.Add(fort.transform);

            }
        }

        /// <summary>
        /// Get the closest walkable node to the specified node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node GetClosestWalkable(Node node)
        {
            Node walkableNode = null;

            int closestCost = 10;

            // Loop 
            for (int x = 5; x >= -5; x--)
            {
                for (int y = 5; y >= -5; y--)
                {
                    if (x == 0 && y == 0)
                        continue; // Skip if the node is the specified node

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    // Check if the node is inside of the grid then return value
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && nodeGrid[checkX, checkY].walkable)
                    {
                        // Get the cost of that node and then make closest
                        int newX = x;
                        int newY = y;

                        if (newX < 0)
                            newX = newX * -1;
                        if (newY < 0)
                            newY = newY * -1;
                        if (closestCost > newX + newY)
                        {
                            closestCost = newX + newY;
                            walkableNode = nodeGrid[checkX, checkY];
                        }

                        if (closestCost == newX + newY)
                        {
                            //if nodeGrid[checkX, checkY] is closer than walkablenode
                            walkableNode = nodeGrid[checkX, checkY];

                        }

                    }
                }
            }
                if (walkableNode == null)
                    print("unable to find a valid node");

                return walkableNode;
            }

            /// <summary>
            /// Refernece a node in the nodeGrid using a world point.
            /// </summary>
            /// <param name="worldPosition"></param>
            /// <returns></returns>
            public Node NodeFromWorldPoint(Vector3 worldPosition)
            {

                //Optimised values for determining the percetange in the grid (uses worldPosition.z not y because of orientation)
                //float percentX = worldPosition.x / gridWorldSize.x + 0.5f; //float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
                //float percentY = worldPosition.z / gridWorldSize.y + 0.5f; //float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
                float percentX = (worldPosition.x - transform.position.x) / gridWorldSize.x + 0.5f - (nodeRadius / gridWorldSize.x);
                float percentY = (worldPosition.z - transform.position.z) / gridWorldSize.y + 0.5f - (nodeRadius / gridWorldSize.y);

                // Prevent the world position from being out of the array if player is out of the grid.
                percentX = Mathf.Clamp01(percentX);
                percentY = Mathf.Clamp01(percentY);

                //Optimised values
                int x = Mathf.FloorToInt(Mathf.Min((gridSizeX * percentX), gridSizeX - 1)); //int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
                int y = Mathf.FloorToInt(Mathf.Min((gridSizeY * percentY), gridSizeY - 1)); //int y = Mathf.RoundToInt((gridSizeY-1) * percentY);


                return nodeGrid[x, y];
            }


            /// <summary>
            /// Displays gizmos in the editor window to show the grid
            /// </summary>
            void OnDrawGizmos()
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
                if (nodeGrid != null && displayGridGizmos)
                {
                    foreach (Node n in nodeGrid)
                    {

                        Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                        Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                    }
                }
            }

        /// <summary>
        /// Class for soring terrain type information
        /// </summary>
        [System.Serializable]
        public class TerrainType
        {
            public LayerMask terrainMask; //Each terrain has a different mask.
            public int terrainPenalty; // Movement penalty for a terrain type.
        }


    }
}