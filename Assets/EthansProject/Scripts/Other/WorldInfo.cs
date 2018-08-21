using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public static class WorldInfo
    {

        public static List<Resource> berryBushes = new List<Resource>();
        public static List<Resource> trees = new List<Resource>();

        public static ResourceSupply berrySorages;
        public static ResourceSupply treeStorages;

        public static List<ResourceSupply> filledStorage = new List<ResourceSupply>();

        public static int globalBerryAmount, globalLogsAmount;
        public static float glogalApititeConsumtionthing;

        public static List<Torch> torches = new List<Torch>();
        public static List<Gatherer> berryGatherers = new List<Gatherer>();

        public static List<Gatherer> woodGatherers = new List<Gatherer>();

        public static float RolePercentage()
        {
            int total = berryGatherers.Count + woodGatherers.Count;

            float percent = berryGatherers.Count / (float)total;

            return percent;
        }
    }
}
