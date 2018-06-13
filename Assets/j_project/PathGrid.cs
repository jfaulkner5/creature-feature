using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace jfaulkner
{

    public class PathGrid : MonoBehaviour
    {
        //Filters the player level to make it easier to find ojects that are considered obsticles
        public LayerMask obstacleMask;

        public Vector2 gridWorldSize;
        public float nodeRad;
        public float nodeDiam;
        public int gridSizeX;
        public int gridSizeY;

        Node[,] levelGrid;

        List<Node> neighbours;
        List<Node> path;


        private void OnDrawGizmos()
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

        private void OnDrawGizmosSelected()
        {

        }

        public void Start()
        {
            nodeDiam = nodeRad * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiam);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiam);

            CreateGrid();
        }

        void CreateGrid()
        {
            levelGrid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBotLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiam + nodeRad) + Vector3.forward * (y * nodeDiam + nodeRad);
                    bool passable = !(Physics.CheckCapsule(worldPoint, worldPoint + new Vector3(0, 1, 0), nodeRad, obstacleMask));
                    //List<Node> neighbours = GetNeighbourNodes(levelGrid[x,y]);

                    levelGrid[x, y] = new Node(passable, worldPoint, x, y);
                }
            }
        }

        #region Untested code

        public List<Node> GetNeighbourNodes(Node node)
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

                    int neighbourCheckX = node.gridX + x;
                    int neighbourCheckY = node.gridY + y;

                    if (neighbourCheckX >= 0 && neighbourCheckX < gridSizeX && neighbourCheckY >= 0 && neighbourCheckY < gridSizeY)
                    {
                        neighbours.Add(levelGrid[neighbourCheckX, neighbourCheckY]);
                    }
                }
            }
            return neighbours;
        }

        #endregion

    }
}
