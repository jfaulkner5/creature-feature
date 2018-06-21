using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class PathFinding
    {
        public List<Node> neighbourNodes;

        public List<Node> FindPath(Vector3 startPos, Vector3 finishPos)
        {
            //Node startNode = ConvertFromWorldPoint(startPos);
            //Node endNode = ConvertFromWorldPoint(finishPos);

            Node startNode = null, endNode = null;
            
            return FindPath(startNode, endNode);
        }

        public List<Node> FindPath(Node startNode, Node endNode)
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

                foreach (Node neighbourNode in GameManager.Instance.GetNeighbourNodes(currentNode))
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
            throw new System.NotImplementedException();
        }

        //private static int GetDistance(Node currentNode, Node neighbour)
        //{
        //    throw new NotImplementedException();
        //}

        int GetDistance(Node nodeA, Node nodeB)
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
    }
}