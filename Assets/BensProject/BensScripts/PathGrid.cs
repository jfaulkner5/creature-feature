using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{

    public class PathGrid : MonoBehaviour
    {             
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadious;
        Node[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        private void Start()
        {
            nodeDiameter = nodeRadious * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            CreateGrid();
        }

        void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBotLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiameter + nodeRadious) + Vector3.forward * (y * nodeDiameter + nodeRadious);
                    bool walkable = !(Physics.CheckCapsule(worldPoint,worldPoint + new Vector3(0,1,0), nodeRadious,unwalkableMask));
                    grid[x, y] = new Node(walkable, worldPoint,x,y);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPoint(Vector3 WorldPosition)
        {
            float percentX = (WorldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (WorldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            return grid[x, y];
        }

        public List<Node> path;

        private void OnDrawGizmos()
        {
            if (grid != null)
            {

                foreach (Node node in grid)
                {
                    Gizmos.color = (node.walkable) ? Color.white : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(node))
                        {
                            Gizmos.color = Color.black;
                        }
                    }      
                    Gizmos.DrawSphere(node.worldPosition,(nodeDiameter/2));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        }
    }

    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;

        public Node parant;

        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }

        public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _grixY)
        {
            walkable = _walkable;
            worldPosition = _worldPosition;
            gridX = _gridX;
            gridY = _grixY;
        }
    }
}