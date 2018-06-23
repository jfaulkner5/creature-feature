using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    public class Resource : MonoBehaviour
    {
        public enum ResourceTypes
        {
            Berry,
            Wood
        }
        public ResourceTypes ResourceType = ResourceTypes.Berry;

        public float regrowthRate;
        public float currentRate;

        public int InitCount = 10;
        public int currentCount;
        public float randomnessPercent = 10;

        public bool hasResource = true;
        public bool debug_Take;

        GameObject Berries
        {
            get { return transform.GetChild(0).gameObject; }
        }

        void AddResource()
        {

            switch (ResourceType)
            {
                case ResourceTypes.Berry:
                    WorldInfo.berryBushes.Add(this);
                    break;
                case ResourceTypes.Wood:
                    WorldInfo.trees.Add(this);

                    break;
                default:
                    break;
            }

        }

        void RemoveResource()
        {

            switch (ResourceType)
            {
                case ResourceTypes.Berry:
                    WorldInfo.berryBushes.Remove(this);
                    break;
                case ResourceTypes.Wood:
                    WorldInfo.trees.Remove(this);

                    break;
                default:
                    break;
            }

        }

        // Use this for initialization
        void Start()
        {
            AddResource();
            regrowthRate *= Random.Range(0.5f, 1.5f);
            currentCount = InitCount;
        }

        // Update is called once per frame
        void Update()
        {
            if (debug_Take)
            {
                debug_Take = false;
                TakeBerries();
            }

            if (currentRate > 0)
            {
                currentRate -= Time.deltaTime;
            }
            else
            {
                if (!hasResource)
                    BerriesRegrew();
            }
        }

        void BerriesRegrew()
        {
            hasResource = true;
            int randChance = (int)Random.Range(-randomnessPercent, randomnessPercent);
            float newBerryCount = InitCount * randChance;
            currentRate = regrowthRate + randChance;
            currentCount = InitCount + Mathf.RoundToInt(randChance);
            AddResource();
            Berries.SetActive(true);
            //  print(currentBerryCount + ", " + randChance);
        }

        public int TakeBerries()
        {
            if (!hasResource)
                return 0;

            hasResource = false;
            currentRate = regrowthRate;

            Berries.SetActive(false);

            RemoveResource();

            return currentCount;


        }
    }
}
