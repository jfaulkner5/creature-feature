﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public static class WorldInfo {

        public static List<Resource> berryBushes = new List<Resource>();
        public static List<Resource> trees = new List<Resource>();

        public static List<ResourceSupply> berrySorages = new List<ResourceSupply>();
        public static List<ResourceSupply> treeStorages = new List<ResourceSupply>();

        public static List<ResourceSupply> filledStorage = new List<ResourceSupply>();

        public static int globalBerryAmount, globalLogsAmount;
        public static float glogalApititeConsumtionthing;
        public static List<Gatherer> berryGatherers = new List<Gatherer>();

        public static List<Gatherer> woodGatherers = new List<Gatherer>();
    }
}