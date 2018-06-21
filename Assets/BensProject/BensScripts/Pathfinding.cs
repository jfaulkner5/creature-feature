﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public static class Pathfinding
    {
        /// <summary>
        /// Finds path from location to location by converting the Locations to Nodes first.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Debug.Log(startPos + " - " + targetPos);

            Node startNode = PathGrid.NodeFromWorldPoint(startPos);
            Node endNode = PathGrid.NodeFromWorldPoint(targetPos);

            if (startNode.walkable && endNode.walkable)
            {
                return FindPath(startNode, endNode);
            }
            Debug.Log("destination not walkable");
            return null;
        }

        /// <summary>
        /// Finds Path from nodes to Node.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        public static List<Node> FindPath(Node startNode, Node endNode)        
        {
            

            List<Node> openSet = PathGrid.permOpen;
            List<Node> closedSet = PathGrid.permClosed;

            openSet.Add(startNode);
            
            while (openSet.Count > 0)
            {

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
                    return RetracePath(startNode, endNode);
                }

                foreach (Node neighbour in PathGrid.GetNeighbours(currantNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMoveCostToNeighbour = currantNode.gCost + GetDistance(currantNode, neighbour);
                    
                    if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
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

            Debug.LogError(closedSet.Count);
            return null;
        }

        static List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currnatNode = endNode;

            while (currnatNode != startNode)
            {
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
