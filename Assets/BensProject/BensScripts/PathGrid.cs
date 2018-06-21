using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{

    public static class PathGrid
    {             
        static LayerMask unwalkableMask;
        public static Vector2 gridWorldSize = new Vector2(16,16);
        static float nodeRadious;
        public static Node[,] grid;
        public static List<Node> permOpen = new List<Node>();
        public static List<Node> permClosed = new List<Node>();

        static float nodeDiameter { get { return nodeRadious * 2; } }
        static int gridSizeX { get { return Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);} }
        static int gridSizeY { get { return Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); } }

        static Transform transform;
        
        public static void CreateGrid(LayerMask _unwalkableMask,int _x,int _z,float _nodeRadious,Transform _transform)
        {
            { // Setup Data
                unwalkableMask = _unwalkableMask;
                gridWorldSize.x = _x;
                gridWorldSize.y = _z;
                nodeRadious = _nodeRadious;

                transform =_transform;
            }            
                        
            grid = new Node[gridSizeX, gridSizeY];
            // Vector3 worldBotLeft = transform.position - Vector3.right * gridWorldSize.x - Vector3.forward * gridWorldSize.y;
            Vector3 worldBotLeft = transform.position + new Vector3(-.5f,0,-0.5f);

            //actualy make the grid
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiameter + nodeRadious) + Vector3.forward * (y * nodeDiameter + nodeRadious);
                    bool walkable = !(Physics.CheckCapsule(worldPoint,worldPoint + new Vector3(0,1,0), nodeRadious - 0.1f,unwalkableMask));                    
                    grid[x, y] = new Node(walkable, worldPoint,x,y);
                    if (walkable)
                    {
                        permOpen.AddSafe(grid[x, y],false);
                    }
                    else
                    {
                        permClosed.AddSafe(grid[x, y],false);
                    }
                }
            }

            // hide zones
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
            //float percentX = (WorldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
            //float percentY = (WorldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
            //percentX = Mathf.Clamp01(percentX);
            //percentY = Mathf.Clamp01(percentY);

            //int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            //int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            int x = Mathf.Clamp(Mathf.RoundToInt(WorldPosition.x),0, gridSizeX -1);
            int y = Mathf.Clamp(Mathf.RoundToInt(WorldPosition.z),0, gridSizeY -1);

            Debug.Log("x: "+ x + " y: " + y);
            Debug.Log("Find " + WorldPosition + " - node " + grid[x, y].worldPosition);
            if (!grid[x, y].walkable)
            {
                List<Node> neighbours = GetNeighbours(grid[x, y]);

                if (neighbours.Count != 0)
                {
                    Node closest = neighbours[0];
                    float distance = Vector3.Distance(neighbours[0].worldPosition, WorldPosition);
                    for (int index = 1; index < neighbours.Count; index++)
                    {
                        float nDist = Vector3.Distance(neighbours[index].worldPosition, WorldPosition);
                        if (nDist < distance)
                        {
                            closest = neighbours[index];
                            distance = nDist;
                        }
                    }
                    return grid[closest.gridX, closest.gridY];
                }
                Debug.LogWarning("found out of walkable");
                return null;
            }

            return grid[x, y];
        }       

        public static Vector3 GetNewLocation()
        {
            return new Vector3(Random.Range(0, gridWorldSize.x - 1), 0, Random.Range(0, gridWorldSize.x - 1));
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
        public bool BackPath = false;

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

        public void Rest()
        {
            BackPath = false;
            parant = null;
            gCost = 0;
            hCost = 0;
        }
    }
}