using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class StoreBerriesAction : GOAPAction {

        public bool droppedOffResource = false;
        ResourceSupply targetResourceSupply;
        public enum GatherType
        {
            BerryGatherer,
            WoodGatherer
        }
        public GatherType CurrGatherType = GatherType.BerryGatherer;
        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }

        public StoreBerriesAction()
        {
            AddPrecondition("hasResource", true);
            AddEffect("hasResource", false);
            AddEffect("collectResource", true);
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            List<ResourceSupply> sources = new List<ResourceSupply>();

            sources = (CurrGatherType == GatherType.BerryGatherer) ? WorldInfo.berrySorages : WorldInfo.treeStorages;
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
            targetResourceSupply = closest;
            return closest != null;
        }

        public override bool IsDone()
        {
            return droppedOffResource;
        }

        public override bool Preform(GameObject agent)
        {
            if (!droppedOffResource && targetResourceSupply.resourceCount < targetResourceSupply.resourceCapacity)
            {
                targetResourceSupply.resourceCount += Storage.resourceHolding;
                // targetResourceSupply.StoreResource(Storage.berriesHolding);
                droppedOffResource = true;
                Storage.resourceHolding = 0;
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
            droppedOffResource = false;
            targetResourceSupply = null;
        }
    }
}

