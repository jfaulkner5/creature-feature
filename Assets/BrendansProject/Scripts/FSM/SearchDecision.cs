using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Search")]
    public class SearchDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool targetFound = Search(controller);
            return targetFound;
        }

        private bool Search(StateController controller)
        {
                Transform bestTarget = null;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = controller.transform.position;
                foreach (Transform potentialTarget in ProcGenerator.instance.targets)
                {
                    Vector3 directionToTarget = potentialTarget.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget;
                    }
                }

            if (bestTarget != null)
            {
                // TODO move into one variable either in unit or controller
                controller.target = bestTarget;
                controller.movingUnit.target = bestTarget;
                controller.movingUnit.StartCoroutine(controller.movingUnit.UpdatePath());
                return true; // found a target so return true
            }
            else
            {
                Debug.Log("Unable to find a target");
                return false; // Didnt find a target.
            }

        }
    }
}