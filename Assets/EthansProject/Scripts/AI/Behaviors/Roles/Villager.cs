using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace EthansProject
{
    public abstract class Villager : MonoBehaviour, IGOAP
    {
        public List<PathingNode> agentPath = new List<PathingNode>();
        public float moveSpeed = 10;
        public Vector3 destination;
        public bool atDestination;

        public float hungerCap = 100;
        public float currentHungerLevel = 100;
        public float foodNeededLevel = 20; // nice naming dickhead
        public float apattiteLevel = .2f;
        public bool needFood;
        public Stopwatch sw;

        public HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //TODO: temp...
        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }


        // Use this for initialization
        void Start()
        {
            sw = new Stopwatch();
            currentHungerLevel = hungerCap;

            apattiteLevel *= Random.Range(0.8f, 1.2f);
            WorldInfo.glogalApititeConsumtionthing += apattiteLevel;

            sw.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if (currentHungerLevel > 0)
                currentHungerLevel -= Time.deltaTime * apattiteLevel;
            else
            {
        
              UnityEngine.Debug.LogError(gameObject.name + " Just died of stavation after, " + sw.Elapsed.Minutes + " minutes, this should be very rare or never happen.");
                Destroy(gameObject);
            }
            if (currentHungerLevel <= foodNeededLevel)
                needFood = true;
            else
                needFood = false;


        }

        public void ActionFinished()
        {
            atDestination = false;

        }

        public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();



        public HashSet<KeyValuePair<string, object>> GetWorldState()
        {
            HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();
            worldData.Add(new KeyValuePair<string, object>("hasResource", (Storage.resourceHolding > 0)));
            worldData.Add(new KeyValuePair<string, object>("expandNeeded", (WorldInfo.filledStorage.Count > 0)));
            worldData.Add(new KeyValuePair<string, object>("dontDieOfHunger", needFood));
            worldData.Add(new KeyValuePair<string, object>("needsFood", needFood));
            // maybe add here a, resource needed thing?!?

            return worldData;
        }

        int index = 0;
        Vector3 currentPos;

        Vector3[] path;
        void CheckPoint()
        {

            currentPos = path[index];
        }

        void StepAgent()
        {
            if (path == null)
            {
                AssignPath();
                return;
            }

            float step = moveSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, currentPos) <= 0.2f)
            {
                if (index < path.Length - 1)
                {
                    index++;
                    CheckPoint();
                }
            }
            else
            {
                if (currentPos != Vector3.zero)
                    transform.position = Vector3.MoveTowards(transform.position, currentPos, step);
            }
        }
        bool needPath = true;
        public bool MoveAgent(GOAPAction nextAction)
        {
            //destination
            //has the destination been reached
            if (!NodeManager.instance._initialized)
            {
                print("havnt inited yet");

                return false;
            }

            destination = nextAction.target.transform.position;

            AssignPath();

            // Don't touch this.
            if (Vector3.Distance(gameObject.transform.position, nextAction.target.transform.position) <= 3.5f)
            {
                // we are at the target location, we are done
                nextAction.SetInRange(true);
                needPath = true;
                index = 0;
                atDestination = true;
                return true;
            }
            else
            {
                StepAgent();

                return false;
            }
        }

        void AssignPath()
        {
            if (!needPath || agentPath == null)
                return;

            needPath = false;
            agentPath = PathingManager.FindPath(gameObject.transform.position, destination);
            path = new Vector3[agentPath.Count];
            for (int i = 0; i < agentPath.Count; i++)
            {
                path[i] = agentPath[i].node.spacialInfo;
            }
            CheckPoint();
        }



        public void PlanAborted(GOAPAction aborter)
        {
            UnityEngine.Debug.LogWarning("Plan aborted by the action: " + aborter);

        }

        public void PlanFailed(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
        {
            UnityEngine.Debug.LogWarning("Plan failed! ");

        }

        public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
        {
            UnityEngine.Debug.LogWarning("Plan found!");


        }
    }
}
