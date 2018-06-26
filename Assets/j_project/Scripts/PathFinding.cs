using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace jfaulkner
{
    public class PathFinding
    {
        public List<Node> neighbourNodes;

        public List<Node> FindPath(Vector3 startPos, Vector3 finishPos)
        {
            //Node startNode = GameManager.Instance.ConvertFromWorldPoint(startPos);
            //Node endNode = GameManager.Instance.ConvertFromWorldPoint(finishPos);
            //Node startNode = GameManager.Instance.levelGrid[0, 0];
            //Node endNode = GameManager.Instance.levelGrid[10, 10];
            throw new System.NotImplementedException();
            //return FindPath(startPos, finishPos);
        }

        public List<Node> FindPath(Node startNode, Node endNode)
        {

            List<Node> openNodeList = new List<Node>();
            List<Node> closedNodeList = new List<Node>();
            List<Node> cameFrom = new List<Node>();
            int gScore = 0; //NOTE setting initial gscore to zero, as there is no current cost from going between start and start.
            int fscore = (int)GetDistance(startNode, endNode); //NOTE setting the start fscore to purely heuristic

            Node currentNode = startNode;
            currentNode.gScore = gScore;
            //TODO should endNode be added to here?
            openNodeList.Add(startNode);

            //TODO needs a loop break for when it can't find the end node
            while (openNodeList != null && currentNode != endNode)
            {
                currentNode = LowestFScore(ref openNodeList);
                if (currentNode == endNode)
                    cameFrom = ReturnPath(cameFrom);


                openNodeList.Remove(currentNode);
                closedNodeList.Add(currentNode);

                foreach (Node neighbour in GetNeighbourNodes(currentNode))
                {
                    if (closedNodeList.Contains(neighbour))
                        continue;
                    if (!openNodeList.Contains(neighbour))
                        openNodeList.Add(neighbour);

                    float tenative_cost = currentNode.gScore + GetDistance(currentNode, neighbour);
                    if (tenative_cost >= neighbour.gScore)
                        continue;

                    cameFrom.Add(neighbour);
                    neighbour.gScore = tenative_cost;
                    neighbour.fScore = gScore + GetDistance(neighbour, endNode);
                }
            }

            //FIX this shouldn't be reached if the path is returned
            throw new System.ArgumentException("Path could not be created");
        }

        //private static int GetDistance(Node currentNode, Node neighbour)
        //{
        //    throw new NotImplementedException();
        //}

        float GetDistance(Node nodeA, Node nodeB)
        {
            return Vector3.Distance(nodeA.worldPos, nodeB.worldPos);
        }

        List<Node> GetNeighbourNodes(Node node)
        {
            List<Node> neighbourNodes = new List<Node>();
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

                    if (nCheckX >= 0 && nCheckX < GameManager.Instance.levelGrid.Length && nCheckY >= 0 && nCheckY < GameManager.Instance.levelGrid.Length)
                    {
                        neighbourNodes.Add(GameManager.Instance.levelGrid[nCheckX, nCheckY]);
                    }

                    //HACK how is this different?
                    //if (nCheckX >= 0 && nCheckX < GameManager.Instance.myPathGrid.gridSize && nCheckY >= 0 && nCheckY < GameManager.Instance.myPathGrid.gridSize)
                    //{
                    //    neighbourNodes.Add(GameManager.Instance.levelGrid[nCheckX, nCheckY]);
                    //}

                }
            }
            return neighbourNodes;
        }

        Node LowestFScore(ref List<Node> openList)
        {
            openList.OrderBy(node => node.fScore).ToList();
            return openList[0];
            //throw new System.NotImplementedException();
        }

        List<Node> ReturnPath(List<Node> _cameFrom)
        {
            Debug.Log("EndNode was reached and return path was triggered");

            _cameFrom.Reverse();
            return _cameFrom;
        }
    }
}