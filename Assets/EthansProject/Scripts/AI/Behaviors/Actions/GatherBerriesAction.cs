using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class GatherBerriesAction : GOAPAction
    {
        public bool hasBerries = false;
        BerryBush targetBerryResource;
        public AgentStorage Storage
        {
            get { return GetComponent<AgentStorage>(); }
        }

        public GatherBerriesAction()
        {
            AddPrecondition("hasBerries", false);
            AddEffect("hasBerries", true);
            
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Berry");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = agent.transform.position;

            foreach (GameObject go in gos)
            {
                 if (!go.GetComponent<BerryBush>().hassBerries)
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
            targetBerryResource = closest.GetComponent<BerryBush>();
            return closest != null;
        }

        public override bool IsDone()
        {
            return hasBerries;
        }

        public override bool Preform(GameObject agent)
        {
            if (!hasBerries && targetBerryResource.hassBerries)
            {
                Storage.berriesHolding += targetBerryResource.TakeBerries();
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
            Storage.berriesHolding = 0;
            hasBerries = false;
            targetBerryResource = null;
            
        }
    }
}
