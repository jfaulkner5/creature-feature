using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jfaulkner
{
    public class AntParent : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public enum AntState
        {
            search,
            scout,
            strengthenPath,
            Hunt,

        }
    }
}