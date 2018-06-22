using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    /// <summary>
    /// Core house of holding states for the GOAP to access and to add.
    /// </summary>
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

        public void PushState(FSMState state)
        {
            stateStack.Push(state);
        }

        public FSMState PopState()
        {
            return stateStack.Pop();
        }
    }
}
