﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Complete;

namespace BrendansProject
{
    /// <summary>
    /// Controls the state of the object it is attached to. Contains information about the current state and stores a reference to the attacking and healing states.
    /// </summary>
    public class StateController : MonoBehaviour
    {

        // State controllers current state
        public State currentState;
        // State controllers previous state
        public State previousState;


        public State remainState; // Dummy state makesing it easier to read in editor. Could just use a null check instead of this state.
        public State chaseState;
        public State attackSearchState;
        public State healSearchState;


        //[HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public MovingUnit movingUnit;
        [HideInInspector] public Unit unit;
        [HideInInspector] public float stateTimeElapsed; // Countdown timer for this controller

        public Transform target; // The transform of the target the unit will travel to(chase).

        public bool aiActive; // state machine active?

        void Awake()
        {

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

            previousState = currentState;

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