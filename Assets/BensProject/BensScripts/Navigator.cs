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


        private void OnDrawGizmos()
        {

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(destination, 1.1f);

            if (path != null && path.Count > 1)
            {
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
            if (path.Count > 0)
            {
                if (Vector3.Distance(transform.position, destination) < .2f)
                {
                    path.RemoveAt(0);
                    if (path.Count == 0)
                    {
                        path = Pathfinding.FindPath(transform.position, PathGrid.GetNewLocation());
                        destination = path[0].worldPosition;
                    }
                    destination = path[0].worldPosition;
                }

                transform.position += transform.forward * speed * Time.deltaTime;
                transform.LookAt(destination, transform.up);
            }
            else
            {
                path = Pathfinding.FindPath(transform.position, PathGrid.GetNewLocation());
                destination = path[0].worldPosition;
            }
        }
    }
}
