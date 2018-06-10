using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{

    /// <summary>
    /// Handles Path Requests, and returns a result.
    /// </summary>
    public static class Pathing
    {

        /// <summary>
        /// The public facing implimentation of the Pathing.
        /// </summary>
        /// <param name="startNode">This is where we path from.</param>
        /// <param name="endNode"> This is where we path to.</param>
        /// <param name="dataSet"> This is what we path through</param>
        /// <returns> Returns a list of PathNodes that is the final path</returns>
        public static List<PathNode> ReturnPath(PathNode startNode, PathNode endNode, List<PathNode> dataSet)
        {
            return AStar(startNode, endNode, dataSet);
        }

        #region First A* Pathing

        /// <summary>
        /// Main A*
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="dataSet"></param>
        /// <returns>Returns a list of PathNodes that is the final path</returns>
        static List<PathNode> AStar(PathNode startNode, PathNode endNode, List<PathNode> dataSet)
        {
            // 1
            PathDataNode start = new PathDataNode(startNode);
            PathDataNode end = new PathDataNode(endNode);
            List<PathNode> returnPath = new List<PathNode>();

            // 2
            List<PathDataNode> OpenList = new List<PathDataNode>();
            start.UpdateFcost(0, end.node);
            OpenList.Add(start);

            //3
            List<PathDataNode> ClosedList = new List<PathDataNode>();

            //4
            while (OpenList.Count > 0)
            {
                PathDataNode currant = OpenList[0];
                for (int index = 1; index < OpenList.Count; index++)
                {
                    if (OpenList[index].F < currant.F  || OpenList[index].F == currant.F && OpenList[index].H < currant.H )
                    {
                        currant = OpenList[index];
                    }
                }

                if (currant == end)
                {
                    PathNode backPath = end.node;
                    while (backPath != start.node)
                    {
                        returnPath.Insert(returnPath.Count, backPath);
                    }

                    return returnPath;
                }

                //5
                foreach (PathDataNode DataNode in currant.nabours)
                {
                    if (ClosedList.Contains(DataNode))
                    {
                        continue;
                    }
                    else if (!OpenList.Contains(DataNode))
                    {
                        DataNode.parant = currant;
                        DataNode.UpdateFcost(currant.G + 1,endNode);
                        OpenList.Add(DataNode);
                    }
                    else if (OpenList.Contains(DataNode))
                    {
                        if (DataNode.G > currant.G + 1)
                        {
                            DataNode.parant = currant;
                            DataNode.G = currant.G + 1;
                        }
                    }
                }
            }
            return null;
        }



        #endregion


    }

    #region Old Path Nodes

    public class PathDataNode
    {
        public PathNode node;
        public PathDataNode parant;
        public List<PathDataNode> nabours = new List<PathDataNode>();
        public float x { get { return node.x; } }
        public float z { get { return node.z; } }

        public float G;
        public float H;
        public float F { get { return G + H; } }
        
        public PathDataNode(PathNode _node)
        {
            node = _node;
            parant = null;
            PopulateNabours();
        }

        void PopulateNabours()
        {
            foreach (PathNode node in node.nabours)
            {
                nabours.Add(new PathDataNode(node));
            }            
        }

        /// <summary>
        /// Updates the F cost by setting the G cost and calculating the huristic.
        /// </summary>
        /// <param name="Gcost"></param>
        /// <param name="currantNode"></param>
        /// <param name="destinationNode"></param>
        public void UpdateFcost(float Gcost, PathNode destinationNode)
        {
            G = Gcost;
            H = Huristic(node, destinationNode);
        }

        /// <summary>
        /// Returns the distance between the two positions.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float Huristic(PathNode start, PathNode end)
        {
            return (end.position - start.position).magnitude;
        }
    }

    public class PathNode
    {
        public float x { get { return position.x; } }
        public float y { get { return position.y; } }
        public float z { get { return position.z; } }
        

        public List<PathNode> nabours = new List<PathNode>();
        public PathMeta meta;

        public Vector3 position;

        /// <summary>
        /// New Node from Vector3
        /// </summary>
        /// <param name="_position"> Node Start position.</param>
        public PathNode(Vector3 _position)
        {
            position = _position;
        }

        /// <summary>
        /// New Node from x,z positions
        /// </summary>
        /// <param name="xPos">start x position</param>
        /// <param name="zPos">start y position</param>
        public PathNode(float xPos,float zPos)
        {
            position.x = xPos;
            position.z = zPos;
        }

        /// <summary>
        /// New Node from x,y,z positions
        /// </summary>
        /// <param name="xPos">start x position</param>
        /// <param name="yPos">start y position</param>
        /// <param name="zPos">start z position</param>
        public PathNode(float xPos,float yPos, float zPos)
        {
            position.x = xPos;
            position.y = yPos;
            position.z = zPos;
        }
    }

    #endregion

    public class PathMeta
    {
        string metaString;
    }

}
