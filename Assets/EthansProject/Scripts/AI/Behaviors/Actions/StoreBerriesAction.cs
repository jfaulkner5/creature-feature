using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class StoreBerriesAction : GOAPAction {

        public bool droppedOffResource = false;
        ResourceSupply targetResourceSupply;
        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }

        public StoreBerriesAction()
        {
            AddPrecondition("hasBerries", true);
            AddEffect("hasBerries", false);
            AddEffect("collectBerries", true);
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Storage");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = agent.transform.position;

            foreach (GameObject go in gos)
            {
                if (go.GetComponent<ResourceSupply>().resourceCount >= go.GetComponent<ResourceSupply>().resourceCapacity)
                    continue;

                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            if (closest == null)
                return false;

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

                targetResourceSupply.resourceCount += Storage.berriesHolding;
                // targetResourceSupply.StoreResource(Storage.berriesHolding);
                droppedOffResource = true;
                Storage.berriesHolding = 0;
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
            Storage.berriesHolding = 0;
            droppedOffResource = false;
            targetResourceSupply = null;

        }
    }
}

