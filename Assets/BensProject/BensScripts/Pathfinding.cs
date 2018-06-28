using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public static class Pathfinding
    {
        static int MaxIntterationCount = 1000;




        /// <summary>
        /// Finds path from location to location by converting the Locations to Nodes first.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            //Debug.Log(startPos + " - " + targetPos);

            Node startNode = PathGrid.NodeFromWorldPoint(startPos);
            Node endNode = PathGrid.NodeFromWorldPoint(targetPos);            
            
            return FindPath(startNode, endNode);
        }

        /// <summary>
        /// Finds Path from nodes to Node.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        public static List<Node> FindPath(Node startNode, Node endNode)        
        {


            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>(PathGrid.permClosed);

            List<Node> returnList = new List<Node>();

            openSet.Add(startNode);

            int itterationcount = 0;

            while (openSet.Count > 0 && itterationcount < MaxIntterationCount)
            {
                ++itterationcount;

                Node currantNode = openSet[0];

                for (int index = 1; index < openSet.Count; index++)
                {
                    //Debug.LogError(index);

                    if (openSet[index].fCost < currantNode.fCost || openSet[index].fCost == currantNode.fCost && openSet[index].hCost < currantNode.hCost)
                    {
                        currantNode = openSet[index];
                    }
                }

                openSet.Remove(currantNode);
                closedSet.Add(currantNode);

                if (currantNode == endNode)
                {
                    returnList = RetracePath(startNode, endNode);
                    break;
                }

                foreach (Node neighbour in currantNode.neighbours)
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMoveCostToNeighbour = currantNode.gCost + GetDistance(currantNode, neighbour);
                    
                    if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMoveCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endNode); // Calculating h cost
                        neighbour.parant = currantNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }                    
                }                
            }

            if (returnList.Count != 0)
            {                
                Reset(openSet, closedSet);
                return returnList;
            }
            else
            {
                Debug.LogWarning("could not find path");
                Reset(openSet, closedSet);
                List<Node> default_ = new List<Node>();
                default_.Add(startNode);
                return default_;
            }            
        }

        static void Reset(List<Node> A, List<Node> B)
        {
            foreach (Node node in A)
            {
                node.Rest();
            }
            foreach (Node node in B)
            {
                node.Rest();
            }
        }

        static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currnatNode = endNode;
            

            while (currnatNode != startNode)
            {                
                currnatNode.BackPath = true;

                path.Add(currnatNode);
                if (currnatNode.parant == null)
                {
                    Debug.LogError(currnatNode.worldPosition);
                }
                currnatNode = currnatNode.parant;
            }

            path.Reverse();
            return path;
        }

        static int GetDistance(Node nodeA, Node nodeB)
        {
            int aX = nodeA.gridX;
            int aY = nodeA.gridY;
            int bX = nodeB.gridX;
            int bY = nodeB.gridY;

            return (Mathf.Abs(aX - bX) + Mathf.Abs(aY - bY));
        }
    }
}
