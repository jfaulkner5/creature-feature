using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Complete;

namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    public class StateController : MonoBehaviour
    {

        // State controllers current state
        public State currentState;

        //public Transform eyes; 

        public State remainState; // Dummy state makesing it easier to read in editor. Could just use a null check instead of this state.
        public State healSearchState;
        public State attackSearchState;


        //[HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public MovingUnit movingUnit;
        [HideInInspector] public Unit unit;
        [HideInInspector] public float stateTimeElapsed; // Countdown timer for this controller

        public Transform target; // The transform of the target the unit will travel to(chase).

        public bool aiActive; // state machine active?

        void Awake()
        {
            //unit = GetComponent<Unit>();

            //TODO check if working
            if (GetComponent<MovingUnit>() != null)
            { 
            movingUnit = GetComponent<MovingUnit>();
            }
            else
            {
                movingUnit = (MovingUnit)GetComponent<Unit>();
            }

            aiActive = true; // activate the state machine
        }

        //public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
        //{
        //    wayPointList = wayPointsFromTankManager;
        //    aiActive = aiActivationFromTankManager;
        //    if (aiActive)
        //    {
        //        navMeshAgent.enabled = true;
        //    }
        //    else
        //    {
        //       navMeshAgent.enabled = false;
        //    }
        //}

        private void Update()
        {
            // If AI is not active return
            if (!aiActive)
                return;

            // Start reference chain to this statecontroller
            currentState.UpdateState(this);

        }

        /// <summary>
        /// Check is its time to transition to a new state
        /// </summary>
        /// <param name="nextState"></param>
        public void TransitionToState(State nextState)
        {
            if (nextState != remainState) // If not remainState or null
            {
                currentState = nextState; // Change states
                                          // OnExitState();
            }
        }

        /// <summary>
        /// A countdown timer using a float as an input
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool CheckIfCountDownElapsed(float duration)
        {
            stateTimeElapsed += Time.deltaTime;
            return (stateTimeElapsed >= duration);
        }

        ///// <summary>
        ///// Reset the timer when the state is exitied.
        ///// </summary>
        private void OnExitState()
        {
            stateTimeElapsed = 0;
        }


    }
}