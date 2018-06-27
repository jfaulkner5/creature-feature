using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BensDroneFleet
{
    [CreateAssetMenu(fileName = "GobjectList", menuName = "ScriptObjects/GameObjectList", order = 0)]
    public class GameObjectList : ScriptableObject
    {
        public List<GameObject> GameObjects = new List<GameObject>();

        public GameObject this[int index]
        {
            get { return GameObjects[index];}
        }

        public void Add(GameObject gObject)
        {
            GameObjects.Add(gObject);
        }

        public void AddSafe(GameObject gObject)
        {
            GameObjects.AddSafe(gObject,false);
        }
    } 
}
