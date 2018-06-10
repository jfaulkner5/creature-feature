using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class Node : MonoBehaviour
    {
        public Node parent;
        public bool isPassable;
        public Vector3 worldPos;
        public int gridX;
        public int gridY;
        public int cost;
        public int hCost;

        public Node(bool _isPassable, Vector3 _worldPos, int _gridX, int _grixY)
        {
            isPassable = _isPassable;
            worldPos = _worldPos;
            gridX = _gridX;
            gridY = _grixY;
        }
    }
}