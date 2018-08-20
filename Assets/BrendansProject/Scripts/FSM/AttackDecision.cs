﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Attack")]
    public class AttackDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool canAttack = QueryUnit(controller);
            return canAttack;
        }

        private bool QueryUnit(StateController controller)
        {

            if (controller.movingUnit.finalLocation & !controller.movingUnit.followingPath & controller.movingUnit.currentHp >= controller.movingUnit.hpAmount / 2) // Check currentHP is above 50% and at final position
                return true;
            else
                return false;
        }
    }
}