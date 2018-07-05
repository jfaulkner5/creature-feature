using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class ObstacleAvoidance : MonoBehaviour
    {
        public float avoidRange = 8f;
        public float avoidanceStrength = 10;

        Transform agent;

        public Vector3 newDir;

        public LayerMask obstacleMask;

        public List<GameObject> currentObstacles = new List<GameObject>();
        // Use this for initialization
        void Start()
        {
            agent = transform.parent;
        }

        // Update is called once per frame
        void Update()
        {

            if (currentObstacles.Count <= 0)
                return;


            Vector3 avoidanceVector = CalculateSeperationVector() * avoidanceStrength;


            agent.position += (avoidanceVector) * Time.deltaTime;

            Debug.DrawLine(transform.position, transform.position + avoidanceVector, Color.blue);

        }

        Vector3 CalculateSeperationVector()
        {
            Vector3 startCenter = Vector3.zero;

            for (int i = 0; i < currentObstacles.Count; i++)
            {
                if (currentObstacles[i])
                    startCenter += agent.position - currentObstacles[i].transform.position;
                else
                    currentObstacles.RemoveAt(i);
            }

            startCenter /= currentObstacles.Count;

            return startCenter.normalized;
        }

        private void OnTriggerEnter(Collider coll)
        {

            if (coll.gameObject.CompareTag("target"))
            {
                if (coll.gameObject != agent)
                    currentObstacles.Add(coll.gameObject);
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("target"))
                if (coll.gameObject != agent)
                    currentObstacles.Remove(coll.gameObject);
        }


    }
}
