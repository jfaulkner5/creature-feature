using UnityEngine;

namespace BrendansProject
{
    /// <summary>
    /// Abstract base class for decisions.
    /// </summary>
    public abstract class Decision : ScriptableObject
    {
        // Returns true or false depending on what is it asked to decide on.
        public abstract bool Decide(StateController controller);
    }
}