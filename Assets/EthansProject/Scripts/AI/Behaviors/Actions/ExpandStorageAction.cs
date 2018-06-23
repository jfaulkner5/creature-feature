using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class ExpandStorageAction : GOAPAction
    {
        public int expandExpences;
        private ResourceSupply targetSupplyToExpand;
        private bool upgraded;

        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }
        public ExpandStorageAction()
        {
            AddPrecondition("hasResource", true);
            AddEffect("hasResource", false);
            AddEffect("expandNeeded", true);
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {

            if (WorldInfo.filledStorage.Count == 0)
            {
                Debug.Log("None to upgrade");
                return false;
            }
            Debug.Log("Some to upgrade");
            List<ResourceSupply> sources = new List<ResourceSupply>();

            sources = WorldInfo.filledStorage;
            ResourceSupply closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = agent.transform.position;

            if (sources.Count == 0)
            {
                Debug.LogError("No resource for " + gameObject.name);
            }


            foreach (ResourceSupply source in sources)
            {

                Vector3 diff = source.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = source;
                    distance = curDistance;
                }
            }
            if (closest == null)
            {
                UnityEngine.Debug.LogWarning("Resource supply not found");

                return false;
            }

            target = closest.gameObject;
            targetSupplyToExpand = closest;
            expandExpences = targetSupplyToExpand.UpgradeCost;
            return closest != null;
        }

        public override bool IsDone()
        {
            return upgraded;
        }

        public override bool Preform(GameObject agent)
        {
            if (!upgraded)
            {

                Storage.resourceHolding -= expandExpences;
                targetSupplyToExpand.UpgradeResources();

                upgraded = true;
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
            upgraded = false;
            targetSupplyToExpand = null;

        }

    }
}
