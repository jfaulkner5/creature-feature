using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace BrendansProject
{

    public class Pathfinding : MonoBehaviour
    {

        public Transform seeker, target;

        LevelGrid levelGrid;

        private void Awake()
        {
            levelGrid = GetComponent<LevelGrid>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                FindPath(seeker.position, target.position);
            }
        }


        void FindPath(Vector3 startPos, Vector3 targetPos)
        {

            Stopwatch sw = new Stopwatch();

            Node startNode = levelGrid.NodeFromWorldPoint(startPos);
            Node targetNode = levelGrid.NodeFromWorldPoint(targetPos);

            Heap<Node> openSet = new Heap<Node>(levelGrid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path Found in " + sw.ElapsedMilliseconds + "ms");

                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in levelGrid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }


        private void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            levelGrid.path = path;

        }


        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (distX > distY)
                return 14 * distX + 10 * (distX - distY);
            return 14 * distX + 10 * (distY - distX);

        }

    }
}