using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{
    [CreateAssetMenu(fileName = "NewResourceList", menuName = "ScriptObjects/ResourceList", order = 0)]
    public class ResourceList : ScriptableObject
    {
        public GameObject[] arrayResourceTypes;
        public List<Resource> listTotal = new List<Resource>();
        public List<Resource> listKnown = new List<Resource>();
        public List<Resource> listReserved = new List<Resource>();
    }
}
