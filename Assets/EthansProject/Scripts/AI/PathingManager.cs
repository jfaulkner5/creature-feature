using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public static class PathingManager
    {
        public static List<PathingNode> openList = new List<PathingNode>();
        public static HashSet<PathingNode> closedList = new HashSet<PathingNode>();



        static NodeManager grid = NodeManager.instance;

        public static void FindPath(Vector3 startPoint, Vector3 endPoint)
        {
           
            PathingNode startNode = grid.NodeFromWorldPoint(startPoint);
            PathingNode endNode = grid.NodeFromWorldPoint(endPoint);
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                //Hack: Un-preforment
                PathingNode currentNode = openList[0];
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].CostF < currentNode.CostF || (openList[i].CostF == currentNode.CostF && openList[i].costG < currentNode.costG))
                    {
                        currentNode = openList[i];
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == endNode)
                {
                    grid.path = RetacePath(startNode, endNode);
                    return;
                }


                foreach (PathingNode neigbouringNode in grid.GetNeigbours(currentNode))
                {

                    if (!neigbouringNode.traverable || closedList.Contains(neigbouringNode))
                        continue;

                    float newNeighbourCost = currentNode.costG + GetDistance(currentNode, neigbouringNode);



                    if (newNeighbourCost < neigbouringNode.costG || !openList.Contains(neigbouringNode))
                    {
                        neigbouringNode.parent = currentNode;

                        neigbouringNode.costG = newNeighbourCost;
                        neigbouringNode.costH = GetDistance(neigbouringNode, endNode);

                        if (!openList.Contains(neigbouringNode))
                        {
                            openList.Add(neigbouringNode);
                        }

                        openList.Add(neigbouringNode);
                    }
                }

            }
        }

        static List<PathingNode> RetacePath(PathingNode startNode, PathingNode endNode)
        {
            List<PathingNode> path = new List<PathingNode>();
            PathingNode currNode = endNode;

            while (currNode != startNode)
            {
                path.Add(currNode);
                currNode = currNode.parent;
            }

            path.Reverse();

            return path;

        }


        static int GetDistance(PathingNode nodeA, PathingNode nodeB)
        {
            int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }

            return 14 * distanceX + 10 * (distanceY - distanceX);

        }

    }


}
