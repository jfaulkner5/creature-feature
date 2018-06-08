using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{

    public static class Pathing
    {
        /*
         * The Goal of this class is to return a path.
         * The simple implimentation will be using basic A*
         */

        /// <summary>
        /// The public facing implimentation of the Pathing.
        /// </summary>
        /// <param name="startNode">This is where we path from.</param>
        /// <param name="endNode"> This is where we path to.</param>
        /// <param name="dataSet"> This is what we path through</param>
        /// <returns></returns>
        public static List<PathNode> ReturnPath(PathNode startNode, PathNode endNode, List<PathNode> dataSet)
        {
            return AStar(startNode, endNode, dataSet);
        }

        static List<PathNode> AStar(PathNode startNode, PathNode endNode, List<PathNode> dataSet)
        {
            Debug.LogError("A* NotImplimented");
            return null;
        }
    }

    public class PathDataNode
    {
        public PathNode node;
        public PathNode parant;
        public float x { get { return node.x; } }
        public float z { get { return node.z; } }
    }

    [System.Serializable]
    public class PathNode
    {
        public float x;
        public float z;

        public List<PathNode> nabours = new List<PathNode>();
        public PathMeta meta;

        public PathNode(float xPos,float zPos)
        {
            Debug.Log("newNode");
            x = xPos;
            z = zPos;
        }
    }

    public class PathMeta
    {
        string metaString;
    }
}
