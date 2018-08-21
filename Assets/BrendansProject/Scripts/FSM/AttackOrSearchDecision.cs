using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackOrSearch")]
    public class AttackOrSearchDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool canAttack = QueryUnit(controller);
            return canAttack;
        }

        private bool QueryUnit(StateController controller)
        {
              if (controller.movingUnit.target != null & controller.movingUnit.target.gameObject.activeSelf) // Check if there is a current target
                return true; // Attack
            else
                return false; // Search
        }
    }
}