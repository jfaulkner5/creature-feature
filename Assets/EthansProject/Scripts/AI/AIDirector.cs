using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class AIDirector : MonoBehaviour
    {

        public int globalBerryAmount, globalLogsAmount, globalBerryGatherers, globalWoodGatherers;
        public float avgDist;
        public float avgSpeed;
        public float averageGatherTime;
        public float averageConsumtionTime = 1;
        public float averageNumberOfBerrysGathered = 15;

        public float checkUpTick = 1f;

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
            StartCoroutine(CheckUp());
        }
        // Use this for initialization
        void RunCheck()
        {
            avgDist = 0;
            avgSpeed = 0;
            averageNumberOfBerrysGathered = 0;

            //Goes and adds all the distancese to all bushes from the storage
            foreach (var berrys in WorldInfo.berryBushes)
            {
                avgDist += Vector3.Distance(WorldInfo.berrySorages[0].transform.position, berrys.transform.position);
                averageNumberOfBerrysGathered += berrys.currentCount;
            }
            // makes thats distance an average
            avgDist /= WorldInfo.berryBushes.Count;
            averageNumberOfBerrysGathered /= WorldInfo.berryBushes.Count;

            foreach (var item in WorldInfo.berryGatherers)
            {
                avgSpeed += item.moveSpeed;
            }


            if (WorldInfo.berryGatherers.Count > 0)
            {
                avgSpeed /= WorldInfo.berryGatherers.Count;
                // calculates the average gather time by using the avgerage distance divided by the average speed times 2 to factor in the return.
                averageNumberOfBerrysGathered /= averageGatherTime = (avgDist / avgSpeed) * 2;
            }
            else
                avgSpeed = averageNumberOfBerrysGathered = averageGatherTime = 0;

            Debug.LogError(averageNumberOfBerrysGathered + " vs " + WorldInfo.glogalApititeConsumtionthing);

            if (averageNumberOfBerrysGathered < (WorldInfo.glogalApititeConsumtionthing) && WorldInfo.woodGatherers.Count > 0)
            {
                int randomWoodGatherer = Random.Range(0, WorldInfo.woodGatherers.Count);
                WorldInfo.woodGatherers[randomWoodGatherer].GetActions();
                RunCheck();
            }
            else if (averageNumberOfBerrysGathered > (WorldInfo.glogalApititeConsumtionthing * 1.2f) && WorldInfo.berryGatherers.Count > 0)
            {
                int randomBerryGatherer = Random.Range(0, WorldInfo.berryGatherers.Count);
                WorldInfo.berryGatherers[randomBerryGatherer].GetActions();
                RunCheck();
            }


            globalBerryGatherers = WorldInfo.berryGatherers.Count;
            globalWoodGatherers = WorldInfo.woodGatherers.Count;
        }

        IEnumerator CheckUp()
        {
            RunCheck();
            yield return new WaitForSeconds(checkUpTick);
            StartCoroutine(CheckUp());
        }

        // Update is called once per frame
        void Update()
        {



        }
    }
}