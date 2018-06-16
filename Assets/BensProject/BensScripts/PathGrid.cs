using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{

    public static class PathGrid
    {             
        static LayerMask unwalkableMask;
        static Vector2 gridWorldSize = new Vector2(16,16);
        static float nodeRadious;
        public static Node[,] grid;

        static float nodeDiameter { get { return nodeRadious * 2; } }
        static int gridSizeX { get { return Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);} }
        static int gridSizeY { get { return Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); } }

        static Transform transform;
        
        public static void CreateGrid(LayerMask _unwalkableMask,int _x,int _z,float _nodeRadious,Transform _transform)
        {
            {
                unwalkableMask = _unwalkableMask;
                gridWorldSize.x = _x;
                gridWorldSize.y = _z;
                nodeRadious = _nodeRadious;
                _transform.position += new Vector3(-8, 0, -8);

                transform = _transform;
            }            

            grid = new Node[gridSizeX, gridSizeY];
            // Vector3 worldBotLeft = transform.position - Vector3.right * gridWorldSize.x - Vector3.forward * gridWorldSize.y;
            Vector3 worldBotLeft = transform.position + new Vector3(-.5f,0,-0.5f);

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiameter + nodeRadious) + Vector3.forward * (y * nodeDiameter + nodeRadious);
                    bool walkable = !(Physics.CheckCapsule(worldPoint,worldPoint + new Vector3(0,1,0), nodeRadious - 0.1f,unwalkableMask));
                    grid[x, y] = new Node(walkable, worldPoint,x,y);
                }
            }
        }

        public static List<Node> GetNeighbours(Node node)
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

        public static Node NodeFromWorldPoint(Vector3 WorldPosition)
        {
            float percentX = (WorldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (WorldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            return grid[x, y];
        }       

    // Requires a path
    //    private static void OnDrawGizmos()
    //    {
    //        if (grid != null)
    //        {

    //            foreach (Node node in grid)
    //            {
    //                Gizmos.color = (node.walkable) ? Color.white : Color.red;
    //                if (path != null)
    //                {
    //                    if (path.Contains(node))
    //                    {
    //                        Gizmos.color = Color.black;
    //                    }
    //                }      
    //                Gizmos.DrawSphere(node.worldPosition,(nodeDiameter/2));
    //            }
    //        }
    //    }

    //    private void OnDrawGizmosSelected()
    //    {
    //        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
    //    }
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