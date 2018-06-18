using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace jfaulkner
{

    public static class PathGrid : MonoBehaviour
    {
        //Filters the player level to make it easier to find ojects that are considered obsticles
        static LayerMask obstacleMask;

        static Vector2 gridWorldSize;
        static float nodeRad;
        static float nodeDiam;
        static int gridSize;

        static   Node[,] levelGrid;

        static List<Node> neighbours;
        static List<Node> path;


        private static void OnDrawGizmos()
        {
            if (levelGrid != null)
            {

                foreach (Node node in levelGrid)
                {
                    Gizmos.color = (node.isPassable) ? Color.grey : Color.red;
                    //if (path != null)
                    //{
                    //    if (path.Contains(node))
                    //    {
                    //        Gizmos.color = Color.green;
                    //    }
                    //}
                    Gizmos.DrawSphere(node.worldPos, (nodeDiam / 2));
                }
            }

        }

        private static void OnDrawGizmosSelected()
        {

        }

        public static void Start()
        {
            nodeDiam = nodeRad * 2;
            gridSize = Mathf.RoundToInt(gridWorldSize.x / nodeDiam);

            CreateGrid();
        }

        public static void CreateGrid()
        {
            levelGrid = new Node[gridSize, gridSize];
           
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiam + nodeRad) + Vector3.forward * (y * nodeDiam + nodeRad);
                    bool passable = !(Physics.CheckCapsule(worldPoint, worldPoint + new Vector3(0, 1, 0), nodeRad, obstacleMask));
                    //List<Node> neighbours = GetNeighbourNodes(levelGrid[x,y]);

                    levelGrid[x, y] = new Node(passable, worldPoint, x, y);
                }
            }
        }

        public static Node ConvertFromWorldPoint(Vector3 worldPoint)
        {
            float posX = (worldPoint.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float posY = (worldPoint.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
            posX = Mathf.Clamp01(posX);
            posY = Mathf.Clamp01(posY);

            int x = Mathf.RoundToInt((gridSize - 1) * posX);
            int y = Mathf.RoundToInt((gridSize - 1) * posY);

            return levelGrid[x, y];
        }
        
        public static List<Node> GetNeighbourNodes(Node node)
        {
            neighbours = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int neighbourCheckX = node.gridPosX + x;
                    int neighbourCheckY = node.gridPosY + y;

                    if (neighbourCheckX >= 0 && neighbourCheckX < gridSize && neighbourCheckY >= 0 && neighbourCheckY < gridSize)
                    {
                        neighbours.Add(levelGrid[neighbourCheckX, neighbourCheckY]);
                    }
                }
            }
            return neighbours;
        }

    }
}
