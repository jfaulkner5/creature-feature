using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/HealOrSearch")]
    public class HealOrSearchDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool canHeal = QueryUnit(controller);
            return canHeal;
        }

        private bool QueryUnit(StateController controller)
        {
            if (controller.movingUnit.target != null & controller.movingUnit.target.gameObject.activeSelf & controller.movingUnit.currentHp < controller.movingUnit.hpAmount) // Check if there is a current target
                return true; // Heal
            else
                return false; // Search
        }
    }
}