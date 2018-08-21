using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class Torch : MonoBehaviour
    {

        public
            float feildRange = 5f, fieldStrength = 5f;


        // Use this for initialization
        void OnEnable()
        {
            WorldInfo.torches.Add(this);
            NodeManager.instance.CalulateLighting(this);
        }
    }
}