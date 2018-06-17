using UnityEngine;


namespace EthansProject
{
    public interface FSMState
    {
        void Update(FSM fsm, GameObject gameObject);
    }
}
