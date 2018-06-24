using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{

    /// <summary>
    /// Base script for each object/unit.
    /// </summary>
    public class Unit : MonoBehaviour
    {

        // Position an ememy will travel to instead of the actial transform position(mainly used for building and humans)
        public Vector3 targetPos;

        public Quaternion targetRot;

        /// <summary>
        /// Display Red gizmo for target node
        /// </summary>
        void OnDrawGizmos()
        {
            if (targetPos != new Vector3(0, 0, 0))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(targetPos, Vector3.one * (0.5f));
            }
        }


    }
}
