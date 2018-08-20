using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
    public class AttackAction : Action
    {
        public override void Act(StateController controller)
        {
            Attack(controller);
        }

        private void Attack(StateController controller)
        {

            Unit targetUnit = null;

            if (controller.target.GetComponent<MovingUnit>() == null)
            {
                targetUnit = controller.target.GetComponent<Unit>();
            }
            else
            {
                targetUnit = controller.target.GetComponent<MovingUnit>();
            }

            // Check if the controller is able to attack using the float from the attackRate
            if (controller.CheckIfCountDownElapsed(controller.movingUnit.tickRate))
            {
                // Attack target
                targetUnit.currentHp -= controller.movingUnit.dmgAmount;

                if (targetUnit.currentHp <= 0)
                {
                    if (targetUnit.CompareTag("Human"))
                    {
                        ProcGenerator.instance.humansList.Remove(targetUnit.transform);
                    }
                    else if (targetUnit.CompareTag("Zombie"))
                    {
                        ProcGenerator.instance.zombiesList.Remove(targetUnit.transform);
                    }
                    else if (targetUnit.CompareTag("Building"))
                    {
                        ProcGenerator.instance.buildingsList.Remove(targetUnit.transform);
                    }

                    targetUnit.gameObject.SetActive(false);
                }

                controller.stateTimeElapsed = 0; // reset state timer
            }
        }
    }
}