using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class ResourceSupply : MonoBehaviour
    {

        public enum StorageTypes
        {
            BerryStorage,
            WoodStorage
        }
        public StorageTypes storageType = StorageTypes.BerryStorage;

        public int resourceCount = 0;
        public int resourceCapacity = 100;
        public float upgradeScale = 1.2f;
        bool isFilled = false;

        public int UpgradeCost
        {
             get { return resourceCapacity; }
        }

        public void StoreResource(int _amount)
        {
            resourceCount += _amount;
        }

        private void Update()
        {            
            if (resourceCount >= resourceCapacity && !isFilled)
            {
                isFilled = true;
                //RemoveStorage();
                WorldInfo.filledStorage.Add(this);
            }
        }

        public int TakeResource(int _amount)
        {
            resourceCount -= _amount;

            return _amount;
        }

        public void UpgradeResources ()
        {
            resourceCapacity += Mathf.RoundToInt(resourceCapacity * upgradeScale);

            Debug.Log("Removed " + this.gameObject);
       
            WorldInfo.filledStorage.Remove(this);
            isFilled = false;

        }

        void AddStorage()
        {
            switch (storageType)
            {
                case StorageTypes.BerryStorage:
                    WorldInfo.globalBerryAmount = this.resourceCount;
                    WorldInfo.berrySorages.Add(this);
                    break;
                case StorageTypes.WoodStorage:
                    WorldInfo.globalLogsAmount = this.resourceCount;
                    WorldInfo.treeStorages.Add(this);
                    break;
                default:
                    break;
            }
        }


        void RemoveStorage()
        {
            switch (storageType)
            {
                case StorageTypes.BerryStorage:
                    WorldInfo.berrySorages.Remove(this);
                    break;
                case StorageTypes.WoodStorage:
                    WorldInfo.treeStorages.Remove(this);
                    break;
                default:
                    break;
            }
        }

        private void Start()
        {
            AddStorage();
        }
    }
}
