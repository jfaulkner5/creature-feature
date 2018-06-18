using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class PathFinding
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static List<Node> FindPath(Vector3 startPos, Vector3 finishPos)
        {
            Node startNode = 
            Node endNode = PathGrid.ConvertFromWorldPoint(finishPos);

            return FindPath(startNode,endNode);
        }

        public static List<Node> FindPath(Node startNode, Node endNode)
        {


            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();

            openSet.Add(startNode);


            return null;
        }

        }
    }