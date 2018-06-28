using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class AIDirector : MonoBehaviour
    {

        public int globalBerryAmount, globalLogsAmount, globalBerryGatherers, globalWoodGatherers;
        private float avgDist;
        public float averageGatherTime;
        public float averageConsumtionTime = 1;
        public float averageNumberOfBerrysGathered = 15;
        // Check the amount of storage
        void CheckStorageCounts()
        {
            // check the amount of agents gatherers 
            globalBerryGatherers = WorldInfo.berryGatherers.Count;
            globalWoodGatherers = WorldInfo.woodGatherers.Count;
        }

        /// <summary>
        /// - gather amount of berrys in storage 
        /// - gather berry gatherers count 
        /// - gather wood gatherers count 
        /// 
        /// check the total amount of consumtion 
        /// 
        /// </summary>

        // Use this for initialization
        void Start()
        {
            //Why? I don't know..
            InvokeRepeating("CheckStorageCounts", 1, 1);

         
            foreach (var berrys in WorldInfo.berryBushes)
            {
                avgDist += Vector3.Distance(WorldInfo.berrySorages[0].transform.position, berrys.transform.position);
            }
            
            avgDist /= WorldInfo.berryBushes.Count;

            averageNumberOfBerrysGathered /= averageGatherTime = (avgDist / 10) * 2;

        }

        // Update is called once per frame
        void Update()
        {



        }
    }
}