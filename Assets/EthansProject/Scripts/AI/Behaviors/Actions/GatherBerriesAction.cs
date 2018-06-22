using UnityEngine;

namespace EthansProject
{
    public class GatherBerriesAction : GOAPAction
    {
        public enum GatherType
        {
            BerryGatherer,
            WoodGatherer
        }
        public GatherType CurrGatherType = GatherType.BerryGatherer;
        public bool hasBerries = false;
        BerryBush targetBerryResource;


        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }

        public GatherBerriesAction()
        {
            AddPrecondition("hasResource", false);
            AddEffect("hasResource", true);

        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            GameObject[] gos;
     
            gos = (CurrGatherType == GatherType.BerryGatherer) ? GameObject.FindGameObjectsWithTag("Berry") : GameObject.FindGameObjectsWithTag("Tree");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = agent.transform.position;
            if (gos.Length == 0)
            {
                Debug.LogError("No resource for " + gameObject.name);
            }


            foreach (GameObject go in gos)
            {
                BerryBush bBush = go.GetComponent<BerryBush>();
                if (bBush == null)
                {
                    UnityEngine.Debug.LogWarning("Resource " + go.name + " did not have the script");
                    continue;
                }
                if (!go.GetComponent<BerryBush>().hasResource)
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
            {
                UnityEngine.Debug.LogWarning("Resource not found");

                return false;
            }

            target = closest;
            targetBerryResource = closest.GetComponent<BerryBush>();
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
