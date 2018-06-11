using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class NavigationSystem : MonoBehaviour {
       
        public Vector3 Target
        {
            get { return GetClosestTarget(); }
        }
        //TODO Stop being trash
        NodeManager grid;
        Vector3 tempTarg;
        // Use this for initialization
        void Start() {
            grid = NodeManager.instance;
            tempTarg = Target;
        }

        // Update is called once per frame
        void Update() {

            PathingManager.FindPath(gameObject.transform.position, tempTarg);
            
            return;
            if (grid.nodes.Count > 0)
            {
                for (int i = 0; i < grid.nodes.Count; i++)
                {
                    while (Vector3.Distance(transform.position, grid.nodes[i].node.spacialInfo) > 0.5f)
                    {
                        transform.position = Vector3.Lerp(transform.position, grid.nodes[i].node.spacialInfo, 0.5f);
                    }
                }
            }
        }
        //TODO Stop being trash
        Vector3 GetClosestTarget()
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Berry");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
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