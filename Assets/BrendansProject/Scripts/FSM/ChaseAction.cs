using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BrendansProject
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
    public class ChaseAction : Action
    {
        public override void Act(StateController controller)
        {

            Chase(controller);
            
        }

        private void Chase(StateController controller)
        {

            if (controller.currentState != controller.previousState)
            {
                controller.movingUnit.StartCoroutine(controller.movingUnit.UpdatePath());
            }

        }
    }
}