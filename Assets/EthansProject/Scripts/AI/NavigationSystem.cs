using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class NavigationSystem : MonoBehaviour {
       
        public Vector3 Target
        {
            get { return GetClosestBerryBush(); }
        }
         
        Vector3 tempTarg;
        // Use this for initialization
        void Start() {
          
        }
        public bool recalculatePath = true;
        // Update is called once per frame
        void Update() {

            if (!NodeManager.instance._initialized)
                return;

            if (!recalculatePath)
                return;

            tempTarg = Target;
            PathingManager.FindPath(gameObject.transform.position, tempTarg);
            recalculatePath = false;
            return;
            // Dosnt work just yet
            if (NodeManager.instance.nodes.Count > 0)
            {
                for (int i = 0; i < NodeManager.instance.nodes.Count; i++)
                {
                    while (Vector3.Distance(transform.position, NodeManager.instance.nodes[i].node.spacialInfo) > 0.5f)
                    {
                        transform.position = Vector3.Lerp(transform.position, NodeManager.instance.nodes[i].node.spacialInfo, 0.5f);
                    }
                }
            }
        }
         /// <summary>
         /// Returns the closest berry bush that has berries.
         /// </summary>
         /// <returns></returns>
        Vector3 GetClosestBerryBush()
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Berry");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (GameObject go in gos)
            {
                if (!go.GetComponent<BerryBush>().hassBerries)
                    continue;

                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                  
                    closest = go;
                    distance = curDistance;
                }
            }
            return closest.transform.position;
        }

    }
    
}