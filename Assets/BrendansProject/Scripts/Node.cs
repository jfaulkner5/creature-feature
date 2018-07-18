using UnityEngine;
using System.Collections;

namespace BrendansProject
{
    /// <summary>
    /// Class Used for storing information about a node.
    /// </summary>
    public class Node : IHeapItem<Node>
    {
        
        public bool walkable;
        
        public Vector3 worldPosition; // Stores the nodes position in the world.

        // X and Y positions in the grid
        public int gridX;
        public int gridY;

        public int movementPenalty;

        // A* Costs
        public int gCost;
        public int hCost;

        public Node parent; // The node considered the parent of this node

        private int heapIndex; // The index number used in the heap

        /// <summary>
        /// Contructor that sets the values of a node on creation.
        /// </summary>
        /// <param name="_walkable"></param>
        /// <param name="_worldPos"></param>
        /// <param name="_gridX"></param>
        /// <param name="_gridY"></param>
        /// <param name="_penalty"></param>
        public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
        {
            walkable = _walkable;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
            movementPenalty = _penalty;
        }

        /// <summary>
        /// Returns the fCost of the node.
        /// </summary>
        public int FCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        /// <summary>
        /// Get and Set the HeapIndex of a node.
        /// </summary>
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

        /// <summary>
        /// Compare the fCosts of this node and a specified node.
        /// </summary>
        /// <param name="nodeToCompare"></param>
        /// <returns></returns>
        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare; // Because nodes are reversed return negative compare. 
        }
    }
}