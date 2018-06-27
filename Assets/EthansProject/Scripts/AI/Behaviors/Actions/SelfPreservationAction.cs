using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class SelfPreservationAction : GOAPAction {

        public bool recivedFood = false;
        ResourceSupply targetResourceSupply;

        public float AmountNeeded
        {
            get { return (Mathf.Abs(GetComponent<Villager>().hungerCap - GetComponent<Villager>().currentHungerLevel)) / 2; }
        }

        public SelfPreservationAction()
        {
            AddPrecondition("needsFood", true);
            AddEffect("needsFood", false);
            AddEffect("stayFull", true);
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            List<ResourceSupply> sources = new List<ResourceSupply>();

            sources =  WorldInfo.berrySorages;
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
            return recivedFood;
        }

        public override bool Preform(GameObject agent)
        {
            if (!recivedFood)
            {
                agent.GetComponent<Villager>().currentHungerLevel += targetResourceSupply.TakeResource(Mathf.RoundToInt(AmountNeeded)) * 2;
                // targetResourceSupply.StoreResource(Storage.berriesHolding);
                recivedFood = true;
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
          
            recivedFood = false;
            targetResourceSupply = null;
        }
    }
}