using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    public class PlaceTorchAction : GOAPAction
    {

        public GameObject torchPrfab;

        public int costToPlace;

        public PlaceTorchAction()
        {
            AddPrecondition("placedTorch", false);
            AddEffect("placedTorch", true);
            AddEffect("lightArea", true);
        }

        public override bool CheckProcPreconditions(GameObject agent)
        {

            // searches for an area with alot of resources in that area and desides to place a torch.
            // TODO: make it place torches that light a path to that torch. - but for now, dont.
            Vector3 torchPlacePoint;
            foreach (var item in WorldInfo.torches)
            {

            }
            return false;

        }

        public override bool IsDone()
        {
            throw new System.NotImplementedException();
        }

        public override bool Preform(GameObject agent)
        {
            throw new System.NotImplementedException();
        }

        public override bool RequiresInRange()
        {
            return true;
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

    }
}
