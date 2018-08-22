using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackOrHeal")]
    public class AttackOrHealDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool canAttack = QueryUnit(controller);
            return canAttack;
        }

        private bool QueryUnit(StateController controller)
        {
            //TODO add a heal amount variable
            if (controller.movingUnit.currentHp > controller.movingUnit.hpAmount / 2) // Check currentHP more than 50% or if being attacked
                return true; // Attack
            else if (controller.movingUnit.gettingAttacked & controller.CompareTag("Zombie"))
            {
                return true; // Attack
            }
            else
            {
                controller.movingUnit.gettingAttacked = false;
                return false; // Heal
            }
        }
    }
}
