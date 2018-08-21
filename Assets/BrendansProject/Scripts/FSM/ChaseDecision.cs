using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Chase")]
    public class ChaseDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool isChasing = QueryUnit(controller);
            return isChasing;
        }

        private bool QueryUnit(StateController controller)
        {

            if (!controller.movingUnit.finalLocation)
            {
                return true; // Continue chasing
            }
            else
            {
                return false; // Goto Attacking/Healing
            }

        }
    }
}