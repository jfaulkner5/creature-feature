using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{
    
    public class Navigator : MonoBehaviour
    {
        public SimpleDroneAI owner;
        
        public float speed = 1f;
        public float acceptableDestinationRange = .1f;
        public float acceptablePathRange = .2f;
        public Vector3 nextLocation;
        public Vector3 endLocation;
        public Vector3 destination;

        public int NodeCount;

        List<Node> path = new List<Node>();

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.cyan;

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
            switch (owner.navState)
            {
                case NavState.Idle:
                    break;
                case NavState.NoPath:
                    break;
                case NavState.NewPath:
                    break;
                case NavState.MoveingTo:
                    break;
                case NavState.AtDestination:
                    owner.AtJobLocation();                    
                    break;
                default:
                    break;
            }
        }

        public void PathFromJob(AiJob job)
        {
            path = new List<Node>(job.Path);
            destination = job.Destination;
            endLocation = destination;
            owner.navState = NavState.MoveingTo;
        }

        void MoveTo()
        {
            if (!(Vector3.Distance(transform.position, endLocation) <= acceptableDestinationRange))
            {
                if (nextLocation != endLocation)
                {
                    if (Vector3.Distance(transform.position, nextLocation) < acceptablePathRange && path.Count > 1)
                    {
                        path.RemoveAt(0);
                        NodeCount = path.Count;
                        nextLocation = path[0].worldPosition;
                    }

                    UpdateStearing();
                }
                else if (nextLocation == endLocation && Vector3.Distance(transform.position, endLocation) <= acceptableDestinationRange)
                {
                    owner.navState = NavState.AtDestination;
                }
                else
                {
                    UpdateStearing();
                } 
            }
            else
            {
                owner.navState = NavState.AtDestination;
            }
        }

        #region Stearing Controles

        /// <summary>
        /// Runs Move and Turn
        /// </summary>
        void UpdateStearing()
        {
            Move();
            Turn();
        }

        /// <summary>
        /// Sets new location for asset
        /// </summary>
        void Move()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        /// <summary>
        /// Sets new rotation for asset
        /// </summary>
        void Turn()
        {
            transform.LookAt(nextLocation, transform.up);
        }

        #endregion

        
    }
}
