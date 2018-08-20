using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Heal")]
    public class HealDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool canHeal = QueryUnit(controller);
            return canHeal;
        }

        private bool QueryUnit(StateController controller)
        {

            if (controller.movingUnit.finalLocation & !controller.movingUnit.followingPath & controller.movingUnit.currentHp < controller.movingUnit.hpAmount) // Check currentHP is below 100% and at final position
                return true;
            else
                return false;
        }
    }
}
