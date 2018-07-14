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
        public float StartingKnowArea = 3f;

        [Header("data")]
        public List<Node> KnownLocations = new List<Node>();
        public List<Resource> KnownResource = new List<Resource>();
        public GameObject DronePrefab;


        // droneState
        List<SimpleDroneAI> OwnedDrones = new List<SimpleDroneAI>();
        public List<AiJob> JobBoard = new List<AiJob>();
        //
        [Header("DebugInfo")]
        public float timeToSpawn;
        public int knownLocationCounter;
        public int UnclamedJobs;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, StartingKnowArea);

            Gizmos.color = Color.black;
            foreach (Node node in KnownLocations)
            {
                Gizmos.DrawWireSphere(node.worldPosition, .2f);
            }
            foreach (Resource resource in KnownResource)
            {
                Gizmos.DrawLine(resource.transform.position, resource.transform.position + resource.transform.up * 3);
            }
        }

        void UpdateDebugInfo()
        {
            if (Application.isEditor)
            {
                knownLocationCounter = KnownLocations.Count;
                UnclamedJobs = JobBoard.Count;
            }
        }

        private void Start()
        {
            SetHome();
            KnownLocations.AddSafe(homeNode, false);
            KnownLocations.AddSafe(homeNode.neighbours[0], false);
            Collider[] startingObj = Physics.OverlapSphere(homeNode.worldPosition, StartingKnowArea);
            foreach (Collider other in startingObj)
            {
                Node node = PathGrid.NodeFromWorldPoint(other.gameObject.transform.position);
                if (node != null && node.walkable)
                {
                    KnownLocations.AddSafe(node, false);
                }
            }

            MakeJobExplore();
            timeToSpawn = 10;
        }

        private void Update()
        {
            UpdateDebugInfo();

            if (timeToSpawn <= 0)
            {
                ConstructDrone();
                timeToSpawn = 10;
            }

            timeToSpawn -= Time.deltaTime;
        }

        void FSM()
        {
            switch (state)
            {
                case DirectorState.Explore:
                    Explore();
                    break;

                case DirectorState.Horde:
                    Horde();
                    break;

                case DirectorState.Populate:
                    Populate();
                    break;

                case DirectorState.Consume:
                    Consume();
                    break;

                default:
                    break;
            }
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
        #region Setup
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
        #endregion
        #region Work 
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
                List<Node> path = new List<Node>();
                path.Add(homeNode);
                nDroneAI.jobCurrant = new AiJob(AiJob.JobType.GetJob, homeNode.worldPosition, null, path);               

                OwnedDrones.Add(nDroneAI);
            }
        }

        void MakeJobExplore()
        {
            Node node = KnownLocations[Random.Range(0, KnownLocations.Count - 1)];
            List<Node> path = Pathfinding.FindPath(homeNode, node);
            if (path.Count > 1)
            {
                JobBoard.Add(new AiJob(AiJob.JobType.Explore, node.worldPosition, null, path));
            }
            else
            {
                Debug.Log("failJob");
            }
        }

        #endregion
    }

    
    [System.Serializable]
    public struct AiJob
    {
        public enum JobType { NoJob,Explore, Collect, GetJob };

        public JobType Job;
        public Vector3 Destination;
        public GameObject Resource;
        public List<Node> Path;

        public AiJob(JobType job,Vector3 destination,GameObject resource,List<Node> path)
        {
            Job = job;
            Destination = destination;
            Resource = resource;
            Path = path;
        }        
    }
}


