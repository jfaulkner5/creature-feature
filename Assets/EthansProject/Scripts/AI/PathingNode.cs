using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class PathingNode
    {
        public float costG, costH;

        public float CostF
        {
            get
            {
                return costH + costG;
            }
        }

        // TODO: what.. public int uniqueID;

        public int gridX, gridY;
        public PathingNode parent;

        public Node node = new Node();
        public bool traverable;
        public PathingNode(bool _traverable, Vector3 _spacialInfo, int x, int y)
        {
            traverable = _traverable;
            node.spacialInfo = _spacialInfo;
            gridX = x;
            gridY = y;
        }
    }
}
