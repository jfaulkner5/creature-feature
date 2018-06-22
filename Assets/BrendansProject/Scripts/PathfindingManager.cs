using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace BrendansProject
{
    /// <summary>
    /// Handles finding a path using A* algorithm
    /// </summary>
    [RequireComponent(typeof(NodeGrid))] // PathfindingManager requires NodeGrid to be on the same gameobject
    public class PathfindingManager : MonoBehaviour
    {

        NodeGrid nodeGrid; // The grid to use for pathfinding

        void Awake()
        {
            nodeGrid = GetComponent<NodeGrid>();
        }

        /// <summary>
        /// Calculaes the shortest path to a target.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void FindPath(PathRequest request, Action<PathResult> callback)
        {

            // Used for debugging the pathfinding time elapsed
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] waypoints = new Vector3[0]; // A vector 3 Array for the waypoints
            bool pathSuccess = false;

            // Start and Target nodes
            Node startNode = nodeGrid.NodeFromWorldPoint(request.pathStart);
            Node targetNode = nodeGrid.NodeFromWorldPoint(request.pathEnd);
            startNode.parent = startNode;

            // Loop through nodes to determine best path
            if (startNode.walkable && targetNode.walkable)
            {
                Heap<Node> openSet = new Heap<Node>(nodeGrid.MaxSize); // Heap(List) of possible nodes to travel
                HashSet<Node> closedSet = new HashSet<Node>(); // List of nodes to ignore
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    // Change pathSuccess to true if a valid path is found
                    if (currentNode == targetNode)
                    {
                        sw.Stop();
                        //print ("Path found: " + sw.ElapsedMilliseconds + " ms");
                        pathSuccess = true;
                        break; // Exit pathfinding loop
                    }

                    foreach (Node neighbour in nodeGrid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        { // Skip if cannot travel to neighbour
                            continue;
                        }

                        // Calculate movement cost of neighbour
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;

                        // Add or update neighbour if lowest gCost
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            // Retrace a succesful path when a path is found
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }

            callback(new PathResult(waypoints, pathSuccess, request.callback));
        }


        /// <summary>
        /// Returns a path of nodes using the node parents.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            // Loop until the start of the path
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;

        }

        /// <summary>
        /// Simplifies the path into waypoints where path changes direction.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 previousDirection = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {

                //Detect if direction changes
                Vector2 newDirection = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

                if (newDirection != previousDirection)
                {
                    waypoints.Add(path[i].worldPosition);
                }

                previousDirection = newDirection;
            }
            return waypoints.ToArray();
        }

        /// <summary>
        /// Calculate the distance using the Manhattan distance
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}