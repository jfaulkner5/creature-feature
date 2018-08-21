using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// Base class for creating a state machine decision.
    /// </summary>
    public abstract class Decision : ScriptableObject
    {
        // Returns true or false depending on what is it asked to decide on.
        public abstract bool Decide(StateController controller);
    }
}