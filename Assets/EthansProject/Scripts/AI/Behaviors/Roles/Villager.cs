using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public abstract class Villager : MonoBehaviour, IGOAP          
    {
        public List<PathingNode> agentPath = new List<PathingNode>();
        public float moveSpeed = 10;
        //TODO: temp...
        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ActionFinished()
        {

        }

        public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();
       

        public HashSet<KeyValuePair<string, object>> GetWorldState()
        {
            HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();
            worldData.Add(new KeyValuePair<string, object>("hasBerries", (Storage.berriesHolding > 0)));
            worldData.Add(new KeyValuePair<string, object>("hasWood", (Storage.logsHolding > 0)));
            return worldData;
        }

        public bool MoveAgent(GOAPAction nextAction)
        {
            float step = moveSpeed * Time.deltaTime;
            agentPath = PathingManager.FindPath(gameObject.transform.position, nextAction.target.transform.position);
            for (int i = 0; i < agentPath.Count; i++)
            {
                while (gameObject.transform.position != agentPath[i].node.spacialInfo)
                {
                    gameObject.transform.position = 
                        Vector3.MoveTowards(gameObject.transform.position, agentPath[i].node.spacialInfo, step);

                }                
            }
            if (Vector3.Distance(gameObject.transform.position, nextAction.target.transform.position) <= 3.5f)
            {
                // we are at the target location, we are done
                nextAction.SetInRange(true);
              
                return true;
            }
            else return false;
        }

          

        public void PlanAborted(GOAPAction aborter)
        {
            agentPath.Clear();

        }

        public void PlanFailed(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
        {

        }

        public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
        {
        }
    }
}
