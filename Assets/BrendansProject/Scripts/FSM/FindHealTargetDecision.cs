using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/HealTargetSearch")]
    public class FindHealTargetDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool targetFound = Search(controller);
            return targetFound;
        }

        private bool Search(StateController controller)
        {

            // Leave decision if unable to attack
            if (controller.movingUnit.currentHp >= controller.movingUnit.hpAmount)
                return false;

            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = controller.transform.position;

            List<List<Transform>> listsToCheck = new List<List<Transform>>();


            if (controller.CompareTag("Human"))
            {

                    listsToCheck.Add(ProcGenerator.instance.buildingsList);

            }
            else if (controller.CompareTag("Zombie"))
            {

                    listsToCheck.Add(ProcGenerator.instance.corpsesList);
            }
            else if (listsToCheck == null)
            {
                Debug.Log("Error no tag combination found");
            }


            foreach (List<Transform> list in listsToCheck)
            {
                foreach (Transform potentialTarget in list)
                {

                    Vector3 directionToTarget = potentialTarget.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget;
                    }
                }
            }


            if (bestTarget != null)
            {
                // TODO move into one variable either in unit or controller REALLLLYYY BAADDD
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