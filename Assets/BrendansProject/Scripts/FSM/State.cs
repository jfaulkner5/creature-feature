using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BrendansProject
{
    /// <summary>
    /// Base class used for creating a state machine state.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/State")]// Used to create Assests in the menu.
    public class State : ScriptableObject
    {

        public Action[] actions; // List of possible actions.

        public Transition[] transitions; // List of possible transitions. Transitions contain decisions but are not decisions

        // Used by set the colour of the gizmos displaying this state.
        public Color sceneGizmoColor = Color.grey;

        /// <summary>
        /// Update state every frame with a specified state controller.
        /// </summary>
        /// <param name="controller"></param>
        public void UpdateState(StateController controller)
        {
            DoActions(controller);
            CheckTransitions(controller);
        }

        /// <summary>
        /// Used to evaluate actions.
        /// </summary>
        /// <param name="controller"></param>
        private void DoActions(StateController controller)
        {
            // Loop through all the actions in the Action array
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Act(controller);
            }
        }

        /// <summary>
        /// Loop through transitions and check their check their decisions
        /// </summary>
        /// <param name="controller"></param>
        private void CheckTransitions(StateController controller)
        {
            // Loops though any transitions in this state
            for (int i = 0; i < transitions.Length; i++)
            {
                // For each transition evaluate each decision and store as decisionSucceeded
                bool decisionSucceeded = transitions[i].decision.Decide(controller);

                if (decisionSucceeded)
                {
                    controller.TransitionToState(transitions[i].trueState); // If the decision succeeds attempt to transition to the true state.
                }
                else
                {
                    controller.TransitionToState(transitions[i].falseState);
                }
            }
        }

    }
}