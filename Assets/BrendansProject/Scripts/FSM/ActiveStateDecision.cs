using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// Check is the taget gameobject is active and returns the result
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/ActiveState")]
    public class ActiveStateDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool targetIsActive = controller.target.gameObject.activeSelf; // Set the active state
            return targetIsActive;
        }
    }
}