using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class PathingNode : IHeapItem<PathingNode>
    {
        public float costG, costH;

        public float CostF
        {
            get
            {
                return costH + costG + node.spacialInfo.y;
            }
        }

        // TODO: what.. public int uniqueID;

        public int gridX, gridY;
        public PathingNode parent;

        public Node node = new Node();
        public bool traverable;
        int heapIndex;
        public PathingNode(bool _traverable, Vector3 _spacialInfo, int x, int y)
        {
            traverable = _traverable;
            node.spacialInfo = _spacialInfo;
            gridX = x;
            gridY = y;
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(PathingNode node )
        {
            int compare = CostF.CompareTo(node.CostF);
            if(compare == 0)
            {
                compare = costH.CompareTo(node.costH);
            }
            return -compare;
        }

    }
}
