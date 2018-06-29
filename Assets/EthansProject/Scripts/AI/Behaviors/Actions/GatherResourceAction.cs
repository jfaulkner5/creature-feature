using UnityEngine;
using System.Collections.Generic;
namespace EthansProject
{
    public class GatherResourceAction : GOAPAction
    {
        public enum GatherType
        {
            BerryGatherer,
            WoodGatherer
        }
        public GatherType CurrGatherType = GatherType.BerryGatherer;
        public bool hasBerries = false;
        Resource targetBerryResource;


        private void Start()
        {
            Add();
        }
        /// <summary>
        /// YIKES what am i doing...
        /// </summary>
        void Add()
        {
            switch (CurrGatherType)
            {
                case GatherType.BerryGatherer:
                   if(!WorldInfo.berryGatherers.Contains(GetComponent<Gatherer>()))
                    WorldInfo.berryGatherers.Add(GetComponent<Gatherer>());

                    if (WorldInfo.woodGatherers.Contains(GetComponent<Gatherer>()))
                        WorldInfo.woodGatherers.Remove(GetComponent<Gatherer>());

                    break;
                case GatherType.WoodGatherer:
                    if (WorldInfo.berryGatherers.Contains(GetComponent<Gatherer>()))
                        WorldInfo.berryGatherers.Remove(GetComponent<Gatherer>());

                    if (!WorldInfo.woodGatherers.Contains(GetComponent<Gatherer>()))
                        WorldInfo.woodGatherers.Add(GetComponent<Gatherer>());
                    break;
                default:
                    break;
            }
        }


        public void SwitchRoles()
        {
            switch (CurrGatherType)
            {
                case GatherType.BerryGatherer:
                    CurrGatherType = GatherType.WoodGatherer;
                    Add();
                    break;
                case GatherType.WoodGatherer:
                    CurrGatherType = GatherType.BerryGatherer;
                    Add();
                    break;
                default:
                    break;
            }
        }
             

        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }

        public GatherResourceAction()
        {
            AddPrecondition("hasResource", false);
            AddEffect("hasResource", true);

        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            List<Resource> sources = new List<Resource>();
            Add();
            sources = (CurrGatherType == GatherType.BerryGatherer) ? WorldInfo.berryBushes : WorldInfo.trees;
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = agent.transform.position;

            if (sources.Count == 0)
            {
                Debug.LogError("No resource for " + gameObject.name);
            }

            foreach (Resource source in sources)
            {
                Resource bBush = source.GetComponent<Resource>();
                if (bBush == null)
                {
                    UnityEngine.Debug.LogWarning("Resource " + source.name + " did not have the script");
                    continue;
                }

                Vector3 diff = source.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = source.gameObject;
                    distance = curDistance;
                }
            }
            if (closest == null)
            {
                UnityEngine.Debug.LogWarning("Resource not found");

                return false;
            }

            target = closest;
            targetBerryResource = closest.GetComponent<Resource>();
            return closest != null;
        }

        public override bool IsDone()
        {
            return hasBerries;
        }

        public override bool Preform(GameObject agent)
        {
            if (!hasBerries && targetBerryResource.hasResource)
            {
              
                Storage.resourceHolding += targetBerryResource.TakeBerries();
              

                hasBerries = true;
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
            hasBerries = false;
            targetBerryResource = null;

        }
    }
}
