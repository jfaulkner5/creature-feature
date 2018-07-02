using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class StoreResourceAction : GOAPAction
    {

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

        public StoreResourceAction()
        {
            AddPrecondition("hasResource", true);
            AddEffect("hasResource", false);
            AddEffect("collectResource", true);
        }

        public void SwitchRoles()
        {
            Debug.Log(gameObject.name + " is now switching roles");
            switch (CurrGatherType)
            {
                case GatherType.BerryGatherer:
                    CurrGatherType = GatherType.WoodGatherer;

                    break;
                case GatherType.WoodGatherer:
                    CurrGatherType = GatherType.BerryGatherer;

                    break;
                default:
                    break;
            }
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            ResourceSupply sources;

            sources = (CurrGatherType == GatherType.BerryGatherer) ? WorldInfo.berrySorages : WorldInfo.treeStorages;
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

                if (AIDirector.instance.RunCheck(CurrGatherType))
                {
                    GetComponent<Gatherer>().GetActions();
                }

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

