using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class SelfPreservationAction : GOAPAction {

        public bool recivedFood = false;
        ResourceSupply targetResourceSupply;

        public float AmountNeeded
        {
            get { return Mathf.Abs(GetComponent<Villager>().hungerCap - GetComponent<Villager>().currentHungerLevel); }
        }
        public SelfPreservationAction()
        {
            AddPrecondition("needsFood", true);
            AddEffect("needsFood", false);
            AddEffect("dontDieOfHunger", true);
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            ResourceSupply sources;

            sources = WorldInfo.berrySorages;
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
            return recivedFood;
        }

        public override bool Preform(GameObject agent)
        {
            if (!recivedFood && targetResourceSupply.resourceCount >= AmountNeeded)
            {
                agent.GetComponent<Villager>().currentHungerLevel += targetResourceSupply.TakeResource(Mathf.RoundToInt(AmountNeeded));
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