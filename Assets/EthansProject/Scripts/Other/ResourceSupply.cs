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


        public int UpgradeCost
        {
             get { return resourceCapacity; }
        }

        public void StoreResource(int _amount)
        {
         
            resourceCount += _amount;

            if (resourceCount >= resourceCapacity)
            {
                Debug.LogError("Added " + this.gameObject);
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


        }

        private void Start()
        {
            switch (storageType)
            {
                case StorageTypes.BerryStorage:
                    WorldInfo.berrySorages.Add(this);
                    break;
                case StorageTypes.WoodStorage:
                    WorldInfo.treeStorages.Add(this);
                    break;
                default:
                    break;
            }
        }
    }
}
