using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet {

    public class PathDataBuilder : MonoBehaviour {

        public bool build = false;
        public int xWidth;
        public int zWidth;
        [Range(1,10)]
        public float resolution = 10;

        [Range(0.1f, 1f)]
        public float nodeHeight;

        [HideInInspector]
        public Vector3 nodeSize { get { return new Vector3(resolution, nodeHeight, resolution); } }

        [HideInInspector]
        public List<PathNode> NodeSet = new List<PathNode>();
        public int NodeCount;

        

        [Header("Gizmo Data")]
        public bool DisplayGizmo;
        public Color GizmoColour = Color.cyan;

        private void OnValidate()
        {
            if (build)
            {
                build = false;

                BuildNodeSet();
            }
        }

        void BuildNodeSet()
        {
            NodeSet.Clear();
            
            for (int x = 0; x < xWidth; x++)
            {
                for (int z = 0; z < zWidth; z++)
                {
                    NodeSet.Add(MakeNewNode(x,z));
                }
            }

            NodeCount = NodeSet.Count;
        }

        PathNode MakeNewNode(int x, int z)
        {
            float newX = x * nodeSize.x;
            float newZ = z * nodeSize.z;

            PathNode node = new PathNode(newX, newZ);
            return node;
        }

        private void OnDrawGizmosSelected()
        {
            if (DisplayGizmo)
            {                
                Gizmos.color = GizmoColour;
                foreach (PathNode node in NodeSet)
                {
                    Gizmos.DrawWireCube(new Vector3(node.x, nodeSize.y * 0.5f, node.z), nodeSize);
                }
            }
        }
    }
}
