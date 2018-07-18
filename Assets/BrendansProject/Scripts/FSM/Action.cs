using UnityEngine;


namespace BrendansProject
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Action : ScriptableObject
    {

        public abstract void Act(StateController controller);
    }
}