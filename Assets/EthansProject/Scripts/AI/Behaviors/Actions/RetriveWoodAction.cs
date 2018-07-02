using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class RetriveWoodAction : GOAPAction
    {
        private ResourceSupply targetResourceSupply;
        private bool hasResource;

        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }

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

            target = closest.gameObject;
            targetResourceSupply = closest;
            return closest != null;
        }
      
        public override bool IsDone()
        {
            return hasResource;
        }

        public override bool Preform(GameObject agent)
        {
            if (!hasResource && targetResourceSupply.resourceCount >= targetResourceSupply.UpgradeCost)
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