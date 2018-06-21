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
            GameObject[] gos;

            gos = (CurrGatherType == GatherType.BerryGatherer) ? GameObject.FindGameObjectsWithTag("BerryStorage") : GameObject.FindGameObjectsWithTag("WoodStorage");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = agent.transform.position;

            foreach (GameObject go in gos)
            {
                if (go.GetComponent<ResourceSupply>().resourceCount >= go.GetComponent<ResourceSupply>().resourceCapacity)
                {
                    UnityEngine.Debug.LogWarning("Resource " + go.name + "was full");
                    continue;
                }

                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            if (closest == null)
            {
                UnityEngine.Debug.LogWarning("Resource storage not found");
                return false;
            }

            target = closest;
            targetResourceSupply = closest.GetComponent<ResourceSupply>();
            return closest != null;
        }

        public override bool IsDone()
        {
            return droppedOffResource;
        }

        public override bool Preform(GameObject agent)
        {
            if (!droppedOffResource)
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

