using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BensDroneFleet
{
    /// <summary>
    /// Explore
    /// - Search environment.
    /// Horde
    /// - Focus on collecting all available resources.
    /// Populate
    /// - Increase population of drones.
    /// Consume
    /// - Decrease population of drones.
    /// </summary>
    public enum DirectorState { Explore, Horde, Populate, Consume }

    public class DirectorAI : MonoBehaviour
    {
        [Header("Director info")]
        public DirectorState state = DirectorState.Explore;
        public Node homeNode;
        public int ResourceE = 100, ResourceR = 10, ResourceG = 10, ResourceB = 10;
        
        [Header("data")]
        public List<Node> KnownLocations = new List<Node>();
        public List<Resource> KnownResource = new List<Resource>();
        public GameObject DronePrefab;
        

        // droneState
        List<SimpleDroneAI> activeDrones = new List<SimpleDroneAI>();
        List<SimpleDroneAI> idleDrones = new List<SimpleDroneAI>();

        //
        float timeToSpawn;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            foreach (Node node in KnownLocations)
            {
                Gizmos.DrawWireSphere(node.worldPosition, .2f);
            }
        }

        private void Start()
        {
            SetHome();            
            KnownLocations.AddSafe(homeNode, false);
            KnownLocations.AddSafe(homeNode.neighbours[0]);
        }

        private void Update()
        {
            if (timeToSpawn <= 0)
            {
                ConstructDrone();
                timeToSpawn = 10;
            }

            timeToSpawn -= Time.deltaTime;
            Debug.Log(timeToSpawn);
        }

        void FSM()
        {
            switch (state)
            {
                case DirectorState.Explore:
                    break;

                case DirectorState.Horde:
                    break;

                case DirectorState.Populate:
                    break;

                case DirectorState.Consume:
                    break;

                default:
                    break;
            }
        }
        
        void StateManager()
        {

        }

        #region states
        void Explore()
        {

        }
        void Horde()
        {

        }
        void Populate()
        {

        }
        void Consume()
        {

        }
        #endregion

        void SetHome()
        {
            Debug.Log("it me");
            homeNode = PathGrid.StartLocation();
            transform.position = new Vector3(homeNode.worldPosition.x, 0, homeNode.worldPosition.z);

            int CheckX = (int)homeNode.worldPosition.x;
            int CheckY = (int)homeNode.worldPosition.z;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0 || x + CheckX == (transform.position + transform.forward).x && y + CheckY == (transform.position + transform.forward).z)
                    {
                        continue;
                    }
                    PathGrid.grid[x + CheckX, y + CheckY].walkable = false;
                    PathGrid.permClosed.Add(PathGrid.grid[x + CheckX, y + CheckY]);
                    foreach (Node node in PathGrid.grid[x + CheckX, y + CheckY].neighbours)
                    {
                        node.neighbours.Remove(PathGrid.grid[x + CheckX, y + CheckY]);
                    }
                }
            }
        }
        
        void SetIdleDrone(SimpleDroneAI drone)
        {
            activeDrones.Remove(drone);
            idleDrones.Add(drone);
        }

        void SetActiveDrone(SimpleDroneAI drone)
        {
            idleDrones.Remove(drone);
            activeDrones.Add(drone);
        }

        void ConstructDrone()
        {
            if (ResourceE >= 10 && ResourceR >= 1 && ResourceG >= 1 && ResourceB >= 1)
            {
                ResourceE -= 10;
                ResourceR -= 1;
                ResourceG -= 1;
                ResourceB -= 1;

                GameObject nDrone = Instantiate<GameObject>(DronePrefab);
                nDrone.transform.SetPositionAndRotation(transform.position, transform.rotation);
                SimpleDroneAI nDroneAI = nDrone.GetComponent<SimpleDroneAI>();

                nDroneAI.director = this;
                SetIdleDrone(nDroneAI);
            }
        }
    }
}

public struct AiJob
{
    Vector3 Destination;
    GameObject Resource;
}
