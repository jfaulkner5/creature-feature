using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public class Pathfinding : MonoBehaviour
    {
        public Transform Seeker;
        public Transform Target;
        public bool updatePath;

        PathGrid grid;

        private void Update()
        {
            if (updatePath)
            {
                updatePath = false;
                grid.path = FindPath(Seeker.position, Target.position);
            }            
        }

        private void Awake()
        {
            grid = GetComponent<PathGrid>();
            if (grid == null)
            {
                Debug.LogError("CurrantlyRequires a built grid on same gameobject");
            }
        }

        List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node endNode = grid.NodeFromWorldPoint(targetPos);

            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();

            openSet.Add(startNode);
            
            while (openSet.Count > 0)
            {

                Node currantNode = openSet[0];
                for (int index = 1; index < openSet.Count; index++) // the problem is that it is looping for < openSet.count but on the first run the count is 1 and it starts at 1. so the loop bales.
                {
                    //Debug.LogError(index);

                    if (openSet[index].fCost < currantNode.fCost || openSet[index].fCost == currantNode.fCost && openSet[index].hCost < currantNode.hCost)
                    {
                        currantNode = openSet[index];
                    }

                    openSet.Remove(currantNode);
                    closedSet.Add(currantNode);

                    if (currantNode == endNode)
                    {                        
                        return RetracePath(startNode, endNode);
                    }

                    foreach (Node neighbour in grid.GetNeighbours(currantNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMoveCostToNeighbour = currantNode.gCost + GetDistance(currantNode, neighbour);
                        if (newMoveCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMoveCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, endNode);
                            neighbour.parant = currantNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }                                
            }

            Debug.LogError("Compleated OpenSet withought finding target. ");
            return null;
        }

        List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currnatNode = endNode;

            while (currnatNode != startNode)
            {
                path.Add(currnatNode);
                currnatNode = currnatNode.parant;
            }

            path.Reverse();
            return path;
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * dstX - dstY;
            }
            else
            {
                return 14 * dstX + 10 * dstY - dstX;
            }
        }
    }
}
