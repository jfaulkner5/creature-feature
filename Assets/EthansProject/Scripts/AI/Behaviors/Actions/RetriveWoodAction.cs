﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class RetriveWoodAction : GOAPAction
    {
        private ResourceSupply targetResourceSupply;
        private bool hasResource = false;

        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }
        public int resourcesNeeded;
        public int ResourceAmountNeeded
        {
            get { return GetComponent<ExpandStorageAction>().expandExpences; }
        }

        public RetriveWoodAction()
        {
            AddPrecondition("hasResource", false);
  
            AddEffect("hasResource", true);


        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            ResourceSupply sources;

            sources = WorldInfo.treeStorages;
            ResourceSupply closest = null;

            Vector3 position = agent.transform.position;

            closest = sources;

            if (closest == null)
            {
                UnityEngine.Debug.LogWarning("Resource supply not found");

                return false;
            }
            resourcesNeeded = GetComponent<ExpandStorageAction>().expandExpences;
            if(closest.resourceCount < ResourceAmountNeeded)
            {
                UnityEngine.Debug.LogWarning("Resource amount contains: " + closest.resourceCount + " and didn't have enough (" + ResourceAmountNeeded + ") to upgrade");

                return false;
            }
            // TODO: check if it gets to this point in the code.
            target = closest.gameObject;
            targetResourceSupply = closest;

            Debug.Log("Closest = " + closest);
            return closest != null;
        }
      
        public override bool IsDone()
        {
            return hasResource;
        }

        public override bool Preform(GameObject agent)
        {
            if (!hasResource && (targetResourceSupply.resourceCount >= ResourceAmountNeeded))
            {
                Storage.resourceHolding += targetResourceSupply.TakeResource(ResourceAmountNeeded);

                hasResource = true;
                return true;
            }
            return false;
        }

        public override bool RequiresInRange()
        {
            return true;
        }

        public override void Reset()
        {
            Storage.resourceHolding = 0;
            hasResource = false;
            targetResourceSupply = null;

        }

    }
}