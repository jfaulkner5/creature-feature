using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{

    [CreateAssetMenu(fileName = "NewMapList", menuName = "ScriptObjects/MapList", order = 0)]
    public class MapList : ScriptableObject
    {
        public List<Texture> list = new List<Texture>();
    }

    [CreateAssetMenu(fileName = "NewDroneList", menuName = "ScriptObjects/SimpleDroneAIList", order = 0)]
    public class SimpleDroneAIList : ScriptableObject
    {
        public List<SimpleDroneAI> listTotal = new List<SimpleDroneAI>();
        public List<SimpleDroneAI> listBusy = new List<SimpleDroneAI>();
        public List<SimpleDroneAI> listIdle = new List<SimpleDroneAI>();
    }

    [CreateAssetMenu(fileName = "NewDroneList", menuName = "ScriptObjects/ResourceList", order = 0)]
    public class ResourceList : ScriptableObject
    {
        public List<Resource> listTotal = new List<Resource>();
        public List<Resource> listKnown = new List<Resource>();
        public List<Resource> listReserved = new List<Resource>();
    }
}