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
                // Attck target
                targetUnit.hpAmount  -=  controller.movingUnit.dmgAmount;

                if (targetUnit.hpAmount <= 0)
                {
                    ProcGenerator.instance.humansList.Remove(targetUnit.transform); // TODO check type and remove from appropriate list
                    targetUnit.gameObject.SetActive(false);
                }

                // Specific target checks (healing)
                if (controller.target.CompareTag("Building") & controller.CompareTag("Zombie"))
                {
                    //controller.movingUnit.hpAmount -= controller.target.GetComponent<MovingUnit>().dmgAmount;
                }
                else if (controller.target.CompareTag("Corpse") & controller.CompareTag("Zombie"))
                {
                    controller.movingUnit.hpAmount += targetUnit.healAmount;
                }

                controller.stateTimeElapsed = 0; // reset attack timer
            }
        }
    }
}