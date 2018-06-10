using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public class PathDataBuilder : MonoBehaviour {

        #region Old Grid Data
        [Header("Generate Old Grid")]
        public bool buildOldGrid = false;
        public bool CheckCount = false;
        public int xWidth;
        public int zWidth;
        [Range(1,10)]
        public float resolution = 10;

        [Range(0.1f, 1f)]
        public float nodeHeight;

        public LayerMask NodeCollisionMask;

        [HideInInspector]
        public Vector3 nodeSize { get { return new Vector3(resolution, nodeHeight, resolution); } }
                
        public PathNode[,] NodeSet;
        public int NodeCount;
        
        [Header("Gizmo Data")]
        public bool DisplayGizmo;
        public Color NodeColour = Color.cyan;
        public Color neighbourColour = Color.blue;

        #endregion        

        private void OnValidate()
        {

            #region Old Grid Triggers
            if (buildOldGrid)
            {
                buildOldGrid = false;

                BuildNodeSet();
            }

            if (CheckCount)
            {
                CheckCount = false;

                foreach (PathNode node in NodeSet)
                {
                    Debug.Log("pos: " + node.position + "   Nabour count:" + node.nabours.Count);
                }
            }

            #endregion

        }

        private void OnDrawGizmos()
        {
            //Old stuff
            #region Old grid Display

            if (DisplayGizmo)
            {
                Gizmos.color = NodeColour;
                if (NodeSet != null && NodeSet.Length > 0)
                {
                    foreach (PathNode node in NodeSet)
                    {
                        if (node != null && node.nabours.Count > 0)
                        {
                            foreach (PathNode nodeB in node.nabours)
                            {
                                Gizmos.DrawLine(node.position, nodeB.position);
                            }
                        }
                        else if (node != null && node.nabours.Count == 0)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(new Vector3(node.x, nodeSize.y * 0.5f, node.z), nodeSize.y);
                        }
                    }
                }
            }

            #endregion
        }

        #region Old Grid Methods

        List<Vector3> GeneratePositionSet(int width, int length, Vector3 positionResolution)
        {
            List<Vector3> returnList = new List<Vector3>();
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    float xPos = (x * positionResolution.x) + transform.position.x;
                    float zPos = (z * positionResolution.z) + transform.position.z;

                    returnList.Add(new Vector3(xPos, positionResolution.y, zPos));
                }
            }

            return returnList;
        }

        void BuildNodeSet()
        {

            //Generate Map positions from size and resolution
            
            //Checks for collison and generates nodes.
            #region Instantiate Nodes

            NodeSet = new PathNode[xWidth, zWidth];

            for (int xpos = 0; xpos < xWidth; xpos++)
            {
                for (int zpos = 0; zpos < zWidth; zpos++)
                {
                    Vector3 location = new Vector3(xpos * nodeSize.x + transform.position.x, 0, zpos * nodeSize.z + transform.position.z);

                    // new node
                    if (!Physics.CheckBox(location, new Vector3(.25f, 1, .25f), transform.rotation, NodeCollisionMask))
                    {
                        PathNode node = MakeNewNode(location);
                        NodeSet[xpos, zpos] = node;

                        #region NodeNabourBackwardsCheck
                        // This Checks the surounding array locations.
                        // And if they are valid will add its self to their neighbour list, and them to its. using list.AddSafe that will only add an object to a list if that list does not allready contain it.
                        // [ xpos, zpos]
                        // [-1, 1] [ 0, 1] [ 1, 1]
                        // [-1, 0] [ 0, 0] [ 1, 0]
                        // [-1,-1] [ 0,-1] [ 1,-1]

                        // [-1, 1]
                        if (xpos - 1 >= 0 && zpos + 1 < zWidth && NodeSet[xpos - 1, zpos + 1] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos - 1, zpos + 1]);
                            NodeSet[xpos - 1, zpos + 1].nabours.AddSafe(node);
                        }
                        // [0, 1]
                        if (xpos >= 0 && zpos + 1 < zWidth && NodeSet[xpos, zpos + 1] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos, zpos + 1]);
                            NodeSet[xpos, zpos + 1].nabours.AddSafe(node);
                        }
                        // [1, 1]
                        if (xpos + 1 < xWidth && zpos + 1 < zWidth && NodeSet[xpos + 1, zpos + 1] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos + 1, zpos + 1]);
                            NodeSet[xpos + 1, zpos + 1].nabours.AddSafe(node);
                        }
                        // [-1, 0]
                        if (xpos - 1 >= 0 && zpos >= 0 && NodeSet[xpos - 1, zpos] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos - 1, zpos]);
                            NodeSet[xpos - 1, zpos].nabours.AddSafe(node);
                        }
                        // [1, 0]
                        if (xpos + 1 < xWidth && zpos >= 0 && NodeSet[xpos + 1, zpos] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos + 1, zpos]);
                            NodeSet[xpos + 1, zpos].nabours.AddSafe(node);
                        }
                        // [-1, -1]
                        if (xpos - 1 >= 0 && zpos - 1 >= 0 && NodeSet[xpos - 1, zpos - 1] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos - 1, zpos - 1]);
                            NodeSet[xpos - 1, zpos - 1].nabours.AddSafe(node);                                     
                        }
                        // [0,-1]
                        if (xpos >= 0 && zpos - 1 >= 0 && NodeSet[xpos , zpos - 1] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos, zpos - 1]);
                            NodeSet[xpos, zpos - 1].nabours.AddSafe(node);
                        }
                        // [1,-1]
                        if (xpos + 1 < xWidth && zpos - 1 >= 0 && NodeSet[xpos + 1, zpos - 1] != null)
                        {
                            node.nabours.AddSafe(NodeSet[xpos + 1, zpos - 1]);
                            NodeSet[xpos + 1, zpos - 1].nabours.AddSafe(node);
                        }

                        #endregion


                    }
                }                              
            }
            NodeCount = NodeSet.Length;

            #endregion

        }

        PathNode MakeNewNode(Vector3 location)
        {
                PathNode node = new PathNode(location.x, location.z);
                
                return node;         
        }
        #endregion

    }
}
