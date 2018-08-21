using UnityEngine;


namespace BrendansProject
{
    /// <summary>
    /// Base class used for creating a state machine action.
    /// </summary>
    public abstract class Action : ScriptableObject
    {

        public abstract void Act(StateController controller);
    }
}