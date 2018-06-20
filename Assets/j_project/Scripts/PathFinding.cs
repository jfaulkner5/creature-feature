using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public static class PathFinding
    {

        public static List<Node> neighbourNodes;

        public static List<Node> FindPath(Vector3 startPos, Vector3 finishPos)
        {
            Node startNode = PathGrid.instance.ConvertFromWorldPoint(startPos);
            Node endNode = PathGrid.instance.ConvertFromWorldPoint(finishPos);


            return FindPath(startNode, endNode);
        }

        public static List<Node> FindPath(Node startNode, Node endNode)
        {

            List<Node> openNodeList = new List<Node>();
            List<Node> closedNodeList = new List<Node>();
            List<Node> cameFrom = new List<Node>();

            //NOTE not neccesary unless there is value per path

            openNodeList.Add(startNode);

            Node currentNode = openNodeList[0];

            while (openNodeList != null && openNodeList.Count != 0)
            {


                //HELP Unsure if tests prove difference or needs to be kept outside the while loop
                //Node currentNode = openSet[0];
                for (int index = 1; index < openNodeList.Count; index++)
                {
                    Debug.Log(index);

                    if (openNodeList[index].F < currentNode.F || openNodeList[index].F == currentNode.F && openNodeList[index].H < currentNode.H)
                    {
                        currentNode = openNodeList[index];


                    }

                }

                openNodeList.Remove(currentNode);
                closedNodeList.Add(currentNode);

                if (currentNode == endNode)
                {

                    currentNode = endNode;

                    while (currentNode != startNode)
                    {
                        cameFrom.Add(currentNode);
                        currentNode = currentNode.parent;
                    }
                    cameFrom.Reverse();
                    return cameFrom;
                }

                foreach (Node neighbourNode in GetNeighbourNodes(currentNode))
                {

                    if (!neighbourNode.isPassable || closedNodeList.Contains(neighbourNode))
                    {
                        continue;
                    }

                    float tempCost = currentNode.G + GetDistance(currentNode, neighbourNode);

                    if (tempCost < neighbourNode.G || !openNodeList.Contains(neighbourNode))
                    {
                        //URGENT fix code to be more fluid
                        neighbourNode.G = tempCost;
                        neighbourNode.H = GetDistance(neighbourNode, endNode);
                        neighbourNode.parent = currentNode;

                        if (!openNodeList.Contains(neighbourNode))
                        {
                            openNodeList.Add(neighbourNode);
                        }
                    }


                }


            }

            return null;
        }

        //private static int GetDistance(Node currentNode, Node neighbour)
        //{
        //    throw new NotImplementedException();
        //}

        static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
            int dstY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * dstX - dstY;
            }
            else
            {
                return 14 * dstX + 10 * dstY - dstX;
            }
        }

        static List<Node> GetNeighbourNodes(Node node)
        {
            neighbourNodes = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int nCheckX = node.gridPosX + x;
                    int nCheckY = node.gridPosY + y;

                    if (nCheckX >= 0 && nCheckX < PathGrid.instance.gridSize && nCheckY >= 0 && nCheckY < PathGrid.instance.gridSize)
                    {
                        neighbourNodes.Add(PathGrid.instance.levelGrid[nCheckX, nCheckY]);
                    }
                }
            }
            return neighbourNodes;
        }

    }
}