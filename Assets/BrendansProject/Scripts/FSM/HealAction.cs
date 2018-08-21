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

                controller.movingUnit.currentHp += targetUnit.healAmount;

                if (targetUnit.CompareTag("Corpse"))
                {
                    targetUnit.currentHp -= controller.movingUnit.dmgAmount;

                    if (targetUnit.currentHp <= 0)
                    {
                        ProcGenerator.instance.corpsesList.Remove(targetUnit.transform);
                        // Do animation here
                        targetUnit.gameObject.SetActive(false);
                    }
                }
                controller.stateTimeElapsed = 0; // reset state timer
            }
        }
    }
}
