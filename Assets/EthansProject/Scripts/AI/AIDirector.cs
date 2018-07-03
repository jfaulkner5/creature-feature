using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class AIDirector : MonoBehaviour
    {

        public static AIDirector instance;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);
            }
            else
            {
                instance = this;
            }
        }

        public int globalBerryAmount, globalLogsAmount, globalBerryGatherers, globalWoodGatherers;
        public float avgDist;
        public float avgSpeed;
        public float averageGatherTime;
        public float averageConsumtionTime = 1;
        public float averageNumberOfBerrysGathered = 15;

        public AnimationCurve scaleRoles;

        public float checkUpTick = 6f;

        /// <summary>
        /// - gather amount of berrys in storage 
        /// - gather berry gatherers count 
        /// - gather wood gatherers count 
        /// 
        /// check the total amount of consumtion 
        /// 
        /// </summary>
        private void Start()
        {
           // StartCoroutine(CheckUp());
        }

        // Use this for initialization
        public bool RunCheck(StoreResourceAction.GatherType gatherType) // pass through weather im a berry or wood gatherer.
        {
            float berryPercent = WorldInfo.berrySorages.ReturnFilledPercentage();
           // float logsPercent = WorldInfo.treeStorages.ReturnFilledPercentage();
            float goalRolePercent = scaleRoles.Evaluate(berryPercent);
            float currentRolePercent = WorldInfo.RolePercentage();

            // return out that we dont need a role switch if goalRolePercent  & currentRolePercent are the same.

            // if  there's way more berry gatherers than i need then we need and im a berry gatherer i need to switch else if im a wood gatherer i need to not switch
            // same witch way less but oposite switching outcomes.

            // have a extra berry gatherer threshold of like 20%.

            globalBerryGatherers = WorldInfo.berryGatherers.Count;
            globalWoodGatherers = WorldInfo.woodGatherers.Count;


            if (currentRolePercent == goalRolePercent)
            {
                return false;
            }
           

            if (gatherType == StoreResourceAction.GatherType.BerryGatherer && currentRolePercent < goalRolePercent)
            {
                return false;
            }
            else if (gatherType == StoreResourceAction.GatherType.WoodGatherer && currentRolePercent < goalRolePercent)
            {
                return true;
            }

            if (gatherType == StoreResourceAction.GatherType.BerryGatherer && currentRolePercent > goalRolePercent * 1.2f)
            {
                return true;
            }
            else if (gatherType == StoreResourceAction.GatherType.WoodGatherer && currentRolePercent > goalRolePercent * 1.2f)
            {
                return false;
            }

            return false;


            //avgDist = 0;
            //avgSpeed = 0;
            //averageNumberOfBerrysGathered = 0;
            ////Goes and adds all the distancese to all bushes from the storage
            //foreach (var berrys in WorldInfo.berryBushes)
            //{
            //    if (true)
            //    {

            //    }
            //    avgDist += Vector3.Distance(WorldInfo.berrySorages[0].transform.position, berrys.transform.position);
            //    averageNumberOfBerrysGathered += berrys.currentCount;
            //}

            //// makes thats distance an average
            //avgDist /= WorldInfo.berryBushes.Count;
            //averageNumberOfBerrysGathered /= WorldInfo.berryBushes.Count;

            //foreach (var item in WorldInfo.berryGatherers)
            //{
            //    avgSpeed += item.moveSpeed;
            //}


            //if (WorldInfo.berryGatherers.Count > 0)
            //{
            //    avgSpeed /= WorldInfo.berryGatherers.Count;
            //    // calculates the average gather time by using the avgerage distance divided by the average speed times 2 to factor in the return.
            //    averageNumberOfBerrysGathered /= averageGatherTime = (avgDist / avgSpeed) * 2;
            //}
            //else
            //    avgSpeed = averageNumberOfBerrysGathered = averageGatherTime = 0;

            //Debug.LogError(averageNumberOfBerrysGathered + " vs " + WorldInfo.glogalApititeConsumtionthing);

            //if (averageNumberOfBerrysGathered < (WorldInfo.glogalApititeConsumtionthing) && WorldInfo.woodGatherers.Count > 1)
            //{
            //    int randomWoodGatherer = Random.Range(0, WorldInfo.woodGatherers.Count);
            //    WorldInfo.woodGatherers[randomWoodGatherer].GetActions();
            //}else if (averageNumberOfBerrysGathered > (WorldInfo.glogalApititeConsumtionthing * 1.2f) && WorldInfo.berryGatherers.Count > 1)
            //{
            //    int randomBerryGatherer = Random.Range(0, WorldInfo.berryGatherers.Count);
            //    WorldInfo.berryGatherers[randomBerryGatherer].GetActions();
            //}

           

        }

        //IEnumerator CheckUp()
        //{
        //    RunCheck();
        //    yield return new WaitForSeconds(checkUpTick);
        //    StartCoroutine(CheckUp());
        //}   
    }
}