﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public abstract class Villager : MonoBehaviour, IGOAP
    {
        public List<PathingNode> agentPath = new List<PathingNode>();
        public float moveSpeed = 10;
        public Vector3 destination;
        public bool atDestination;
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
            atDestination = false;

        }

        public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();


        public HashSet<KeyValuePair<string, object>> GetWorldState()
        {
            HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();
            worldData.Add(new KeyValuePair<string, object>("hasResource", (Storage.resourceHolding > 0)));
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
                if(currentPos != Vector3.zero)
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
            Debug.LogWarning("Plan aborted by the action: " + aborter);

        }

        public void PlanFailed(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
        {
            Debug.LogWarning("Plan failed!");

        }

        public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
        {
            Debug.LogWarning("Plan found!");


        }
    }
}
