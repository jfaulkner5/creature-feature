using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
namespace EthansProject
{
    public class ResourceSupply : MonoBehaviour
    {

        public int resourceCount = 0;
        public int resourceCapacity = 100;
       
        public void StoreResource(int _amount)
        {
            resourceCount += _amount;
        }
    }
}
