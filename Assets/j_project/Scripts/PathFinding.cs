using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace jfaulkner
{
    [CreateAssetMenu(fileName = "MyPathFindingInstance", menuName ="PathFinding/AStar")]
    public class PathFinding : ScriptableObject
    {
        public enum PathFindStates
        {
            StartNode,
            Auxiliary,
            GetNeighbours,
            EndNode,
            ReturnPath
        }

        public List<Node> neighbourNodes;
        public List<Node> list;

        public PathFindStates currentFinderState;

        public List<Node> GetList(Node startNode, Node endNode)
        {
            FindPath(startNode, endNode);

            return list;
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 finishPos)
        {

            throw new System.NotImplementedException();
            //return FindPath(startPos, finishPos);
        }

        public List<Node> FindPath(Node startNode, Node endNode)
        {
            //initial clean up to make sure there isn't extra data messing up new path searching
            GameManager.Instance.myPathGrid.Cleanup();
            
            List<Node> openNodeList = new List<Node>();
            List<Node> closedNodeList = new List<Node>();
            int gScore = 0; //NOTE setting initial gscore to zero, as there is no current cost from going between start and start.
            int fscore = (int)GetDistance(startNode, endNode); //NOTE setting the start fscore to purely heuristic

            Node currentNode = startNode;
            currentNode.gScore = gScore;
            //TODO should endNode be added to here?
            openNodeList.Add(startNode);

            //TODO needs a loop break for when it can't find the end node
            while (openNodeList.Count > 0 && currentNode != endNode)
            {
                currentNode = LowestFScore(openNodeList);
                if (currentNode == endNode)
                    return ReturnPath(startNode, endNode);

                openNodeList.Remove(currentNode);
                closedNodeList.Add(currentNode);

                foreach (Node neighbour in GetNeighbourNodes(currentNode))
                {
                    if (closedNodeList.Contains(neighbour))
                        continue;

                    float tenative_cost = currentNode.gScore + GetDistance(currentNode, neighbour);
                    if (!openNodeList.Contains(neighbour))
                        openNodeList.Add(neighbour);

                    else
                    {
                        if (tenative_cost >= neighbour.gScore)
                            continue;
                    }


                    neighbour.parent = currentNode;
                    //cameFrom.Add(neighbour);
                    neighbour.gScore = tenative_cost;
                    neighbour.fScore = gScore + GetDistance(neighbour, endNode);
                }
            }

            //FIX this shouldn't be reached if the path is returned
            //throw new ArgumentException(message: "You shouldn't see this message.");
            return null;
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
                    int levelGridLength = (GameManager.Instance.myPathGrid.gridSize) - 1;
                    if (nCheckX >= 0 && nCheckX < levelGridLength && nCheckY >= 0 && nCheckY < levelGridLength)
                    {
                        if (GameManager.Instance.levelGrid[nCheckX, nCheckY].isPassable)
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

        Node LowestFScore(List<Node> openList)
        {
            return openList.OrderBy(node => node.fScore).ToList()[0];
            //return openList[0];
            //throw new System.NotImplementedException();
        }

        List<Node> ReturnPath(Node _startNode, Node _endNode)
        {
            Debug.Log("EndNode was reached and return path was triggered");

            List<Node> selectedPath = new List<Node>();
            Node currentNode = _endNode;

            while (currentNode != null)
            {
                selectedPath.Add(currentNode);
                currentNode = currentNode.parent;
            }
            selectedPath.Reverse();

            return selectedPath;
        }
    }
}