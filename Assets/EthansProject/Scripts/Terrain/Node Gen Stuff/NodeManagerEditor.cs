using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EthansProject
{
    [CustomEditor(typeof(NodeManager))]
    public class GridManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NodeManager grid = FindObjectOfType<NodeManager>();
            TerrainGen terrGen = FindObjectOfType<TerrainGen>();

            if (GUILayout.Button("Update AI"))
            {
                terrGen.GenerateTerrain();
            }
             
            if (GUILayout.Button("Debug PathData: " + grid.debugMode))
            {
                if (grid.debugMode)
                    grid.debugMode = false;
                else
                    grid.debugMode = true;

            }
            base.DrawDefaultInspector();
        }
    }
}