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
        List<SimpleDroneAI> activeDrones = new List<SimpleDroneAI>();
        public List<AiJob> JobBoard = new List<AiJob>();
        //
        [Header("Timers")]
        public float consumeCounter;
        public float consumeTime;
        public bool droneBuildActive;

        // counters
        int countKnownResource { get { return KnownLocations.Count; } }
        int countKnownLocation { get { return KnownLocations.Count; } }
        int countActiveDrones { get { return activeDrones.Count; } }
        int countActiveJobs { get { return JobBoard.Count; } }
        int desiredResourceValue;

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

            desiredResourceValue = ResourceE + ((ResourceR + ResourceG + ResourceB) * 10);
        }

        private void Update()
        {
            UpdateDebugInfo();

            DroneCounter();
        }        

        void FSM()
        {
            

            switch (state)
            {
                case DirectorState.Explore:
                    // the default, if their are no resources, populate joblist with, explore jobs.


                    Explore();
                    break;

                case DirectorState.Horde:
                    // Each of the RGB resources is worth 10 each, The E resource is worth 1 each.
                    // default resource worth is 400.
                    // If worth is less than 300.

                    Horde();
                    break;

                case DirectorState.Populate:
                    // if their are drones + 1 to drones *2 worth of jobs.
                    
                    Populate();
                    break;

                case DirectorState.Consume:
                    // if their are more drones * 2 than there are jobs for 30s.
                    
                    Consume();
                    break;

                default:
                    break;
            }
        }

        #region states
        
        void Explore()
        {
            Node node = KnownLocations[Random.Range(0, KnownLocations.Count - 1)];
            List<Node> path = Pathfinding.FindPath(homeNode,node);

            JobBoard.Add(new AiJob(AiJob.JobType.Explore, node.worldPosition,null, path));
        }
        void Horde()
        {
            Resource resource = KnownResource[Random.Range(0, KnownResource.Count - 1)];
            if (resource.state == ResourcesState.Reserved)
            {
                return;
            }
            else
            {
                resource.state = ResourcesState.Reserved;
            }

            List<Node> path = Pathfinding.FindPath(homeNode.worldPosition, resource.gameObject.transform.position);
                        
            JobBoard.Add(new AiJob(AiJob.JobType.Collect, resource.gameObject.transform.position, resource.gameObject, path));
        }
        void Populate()
        {
            ActivateDroneCounter();
        }
        void Consume()
        {
            DeactivateDroneCounter();
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

                activeDrones.Add(nDroneAI);
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

        void DroneCounter()
        {
            if (timeToSpawn <= 0)
            {
                droneBuildActive = false;
                ConstructDrone();
            }

            if (droneBuildActive)
            {
                timeToSpawn -= Time.deltaTime;
            }            
        }

        void ActivateDroneCounter()
        {
            timeToSpawn = 10;
            droneBuildActive = true;
        }

        void DeactivateDroneCounter()
        {
            droneBuildActive = false;
        }


        /// <summary>
        /// Low priority is key
        /// </summary>
        /// <returns></returns>
        float HordeUtility()
        {
            int currantResourceValue = ResourceE + ((ResourceR + ResourceG + ResourceB) * 10);
            
            return currantResourceValue / desiredResourceValue;
        }

        float ExploreResourceValue()
        {
            
            return
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


