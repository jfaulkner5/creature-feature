using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{
    public class Navigator : MonoBehaviour
    {
        public float speed = 1f;
        public Vector3 destination;
        List<Node> path = new List<Node>();


        private void OnDrawGizmosSelected()
        {
            if (path != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(path[path.Count - 1].worldPosition, 0.5f);

                for (int i = 0; i < path.Count; i++)
                {
                    if (i + 1 < path.Count)
                    {
                        //Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
                        Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
                    }
                }
            }
        }
        
        private void Update()
        {
            if (path != null)
            {
                if (Vector3.Distance(transform.position, destination) < .2f && path.Count - 1 > 0)
                {
                    path.RemoveAt(0);
                    destination = path[0].worldPosition;
                }

                if (path.Count > 0)
                {
                    transform.position += transform.forward * speed * Time.deltaTime;
                    transform.LookAt(destination, transform.up);
                }
                else
                {
                    path = Pathfinding.FindPath(transform.position, GetRandPos());
                }
            }
            else
            {
                Debug.Log("Path Null");
                path = Pathfinding.FindPath(transform.position, GetRandPos());
            }           
        }

        Vector3 GetRandPos()
        {
            return new Vector3(GetRandom(), 0, GetRandom());
        }

        float GetRandom()
        {
            return Random.Range(0, PathGrid.gridWorldSize.x);
        }
    }
}
