using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace EthansProject
{
    public class FSM
    {
        private Stack<FSMState> stateStack = new Stack<FSMState>();
        public delegate void FSMState(FSM fsm, GameObject gameObject);
        public void Update(GameObject gameObject)
        {

            FSMState peekState = stateStack.Peek();

            if (peekState != null)
                peekState.Invoke(this, gameObject);

        }

        public void pushState(FSMState state)
        {
            stateStack.Push(state);
        }

        public void popState()
        {
            stateStack.Pop();
        }
    }
}
