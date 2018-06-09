using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public class PathDataBuilder : MonoBehaviour {

        public bool build = false;
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

        private void OnValidate()
        {
            if (build)
            {
                build = false;

                BuildNodeSet();
            }

            if (CheckCount)
            {
                CheckCount = false;

                foreach (PathNode node in NodeSet)
                {
                    Debug.Log("pos: " + node.position + "   Nab count: " + node.nabours.Count);
                }
            }
        }

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

                        #region NodeNabourBackwardsCheck
                        // This Checks the three array locations [xPos - 1, zPos] [xPos, zPos - 1]  [xPos - 1, zPos - 1]
                        // And if they are valid will add its self to their neighbour list

                        if (xpos - 1 >= 0)
                        {
                            if (NodeSet[xpos - 1, zpos] != null)
                            {
                                node.nabours.AddSafe(NodeSet[xpos - 1, zpos]);
                                NodeSet[xpos - 1, zpos].nabours.AddSafe(node);
                            }                   
                        }

                        if (xpos + 1 < xWidth)
                        {
                            if (NodeSet[xpos + 1,zpos] != null)
                            {                               
                                node.nabours.AddSafe(NodeSet[xpos + 1, zpos]);
                                NodeSet[xpos - 1, zpos].nabours.AddSafe(node);
                            }
                        }

                        if (xpos - 1 >= 0 && zpos - 1 >= 0)
                        {
                            if (NodeSet[xpos - 1, zpos - 1] != null)
                            {
                                node.nabours.AddSafe(NodeSet[xpos - 1, zpos - 1]);
                                NodeSet[xpos - 1, zpos - 1].nabours.AddSafe(node);
                            }                            
                        }

                        if (xpos + 1 < 0 && xWidth + 1 < zWidth)
                        {
                            if (NodeSet[xpos + 1, zpos + 1] != null)
                            {
                                node.nabours.AddSafe(NodeSet[xpos + 1, zpos + 1]);
                                NodeSet[xpos + 1, zpos + 1].nabours.AddSafe(node);
                            }
                        }

                        if (zpos - 1 >= 0)
                        {
                            if (NodeSet[xpos, zpos - 1] != null)
                            {
                                node.nabours.AddSafe(NodeSet[xpos, zpos - 1]);
                                NodeSet[xpos, zpos - 1].nabours.AddSafe(node);
                            }                            
                        }

                        if (zpos + 1 < zWidth)
                        {                            
                            if (NodeSet[xpos, zpos + 1] != null)
                            {
                                
                                node.nabours.AddSafe(NodeSet[xpos, zpos + 1]);
                                NodeSet[xpos, zpos + 1].nabours.AddSafe(node);
                            }
                        }

                        #endregion

                        NodeSet[xpos, zpos] = node;
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

        private void OnDrawGizmos()
        {
            if (DisplayGizmo)
            {                
                Gizmos.color = NodeColour;
                if (NodeSet != null && NodeSet.Length > 0)
                {
                    foreach (PathNode node in NodeSet)
                    {
                        if (node != null)
                        {
                            Gizmos.DrawSphere(new Vector3(node.x, nodeSize.y * 0.5f, node.z), nodeSize.y);

                            if (node.nabours.Count > 0)
                            {
                                foreach (PathNode nodeB in node.nabours)
                                {
                                    Gizmos.DrawLine(node.position, nodeB.position);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
