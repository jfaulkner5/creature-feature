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

        public int uniqueID;

        public PathingNode parent;

    }
}
