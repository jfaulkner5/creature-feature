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
        public bool traverable, isLit = false;
        public float spacialLighting = 0;
        int heapIndex;

        public PathingNode(bool _traverable, Vector3 _spacialInfo, int x, int y)
        {
            traverable = _traverable;
            node.spacialInfo = _spacialInfo;
            gridX = x;
            gridY = y;
        }

        public void AssignField(PathingNode parent, Torch source)
        {
            //Hack: temp. Want to make it a bit smarter

            if (isLit)
                return;

            if (parent == null)
                spacialLighting = source.fieldStrength;
            else
                spacialLighting = (parent.spacialLighting - 1);

            if (spacialLighting > 0)
            {
                isLit = true;
                Debug.Log("Node at: " + node.spacialInfo + ", has been lit up dude. With a value of: " + spacialLighting);
            }
            else
                return;

            List<PathingNode> subNeigbours = NodeManager.instance.GetNeigbours(this);
            for (int i = 0; i < subNeigbours.Count; i++)
            {
                subNeigbours[i].AssignField(this, source);
            }
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
