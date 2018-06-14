using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BrendansProject
{
    public class PathfindingManager : MonoBehaviour
    {

        public Transform seeker, target;

        LevelGrid levelGrid;

        void Awake()
        {
            levelGrid = GetComponent<LevelGrid>();
        }

        void Update()
        {
            

            if (Input.GetButton("Jump"))
            {
                FindPath(seeker.position, target.position);
            }

        }

        void FindPath(Vector3 startPos, Vector3 targetPos)
        {

            Node startNode = levelGrid.GetNodeFromWorldPoint(startPos);
            Node targetNode = levelGrid.GetNodeFromWorldPoint(targetPos);

            Heap<Node> openSet = new Heap<Node>(levelGrid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
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
                            openSet.Add(neighbour);
                        else
                        {
                            //openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        void RetracePath(Node startNode, Node endNode)
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
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }


    }
}