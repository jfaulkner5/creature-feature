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

            // Check if the controller is able to attack using the float from the attackRate
            if (controller.CheckIfCountDownElapsed(controller.movingUnit.tickRate))
            {
                controller.target.GetComponent<MovingUnit>().hpAmount  -=  controller.GetComponent<MovingUnit>().dmgAmount;
                if (controller.target.CompareTag("Building") & controller.CompareTag("Zombie"))
                {
                    controller.movingUnit.hpAmount -=  controller.target.GetComponent<MovingUnit>().dmgAmount;
                }
                controller.stateTimeElapsed = 0; // reset attack timer
            }
        }
    }
}