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
        public Vector3 lastLocation;
        public Vector3 destination;
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
                    owner.state = SimpleAIState.Wander;
                    break;

                case NavState.NewPath:
                    path = Pathfinding.FindPath(transform.position, destination);
                    if (path.Count > 1)
                    {
                        owner.navState = NavState.MoveingTo;
                        nextLocation = path[0].worldPosition;
                        lastLocation = path[path.Count - 1].worldPosition;
                    }
                    else
                    {
                        owner.navState = NavState.NoPath;
                        owner.state = SimpleAIState.Idle;
                    }
                    break;

                case NavState.MoveingTo:
                    MoveTo();
                    break;
                case NavState.AtDestination:
                    owner.state = SimpleAIState.Idle;
                    break;
                default:
                    break;
            }
        }

        public void SetDestination(Vector3 Dnew)
        {
            destination = Dnew;
            owner.navState = NavState.NewPath;
        }

        void MoveTo()
        {
            if (nextLocation != lastLocation)
            {
                if (Vector3.Distance(transform.position, nextLocation) < acceptablePathRange && path.Count > 1)
                {
                    path.RemoveAt(0);
                    nextLocation = path[0].worldPosition;                           
                }

                UpdateStearing();
            }
            else if (nextLocation == lastLocation && Vector3.Distance(transform.position, lastLocation) <= acceptableDestinationRange)
            {
                owner.navState = NavState.AtDestination;
            }
            else
            {
                UpdateStearing();
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
