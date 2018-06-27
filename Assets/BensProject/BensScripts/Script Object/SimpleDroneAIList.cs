using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{
    [CreateAssetMenu(fileName = "NewDroneList", menuName = "ScriptObjects/SimpleDroneAIList", order = 0)]
    public class SimpleDroneAIList : ScriptableObject
    {
        public List<SimpleDroneAI> listTotal = new List<SimpleDroneAI>();
    }
}
