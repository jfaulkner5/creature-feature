using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Heal")]
    public class HealAction : Action
    {
        public override void Act(StateController controller)
        {
            Heal(controller);
        }

        private void Heal(StateController controller)
        {

            Unit targetUnit = null;

            targetUnit = controller.target.GetComponent<Unit>();

            if (controller.CheckIfCountDownElapsed(controller.movingUnit.tickRate))
            {

                targetUnit.currentHp -= controller.movingUnit.dmgAmount;
                controller.movingUnit.currentHp += targetUnit.healAmount;

                if (targetUnit.currentHp <= 0)
                {
                    if (targetUnit.CompareTag("Corpse"))
                    {
                        ProcGenerator.instance.corpsesList.Remove(targetUnit.transform); // TODO check if should be game object or transform
                        // Do animation here
                    }
                    else if (targetUnit.CompareTag("Building"))
                    {
                        ProcGenerator.instance.buildingsList.Remove(targetUnit.transform);
                        // TODO Spawn any humans in building in attack mode
                        // Do animation here
                    }
                    targetUnit.gameObject.SetActive(false);
                }
                controller.stateTimeElapsed = 0; // reset state timer
            }

        }
    }
}
