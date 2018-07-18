using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/NoTarget")]
    public class NoTargetAction : Action
    {

        public override void Act(StateController controller)
        {
            Message(controller);
        }


        private void Message(StateController controller)
        {
            Debug.Log("No Target Found! In NoTargetState");
        }

    }
}
