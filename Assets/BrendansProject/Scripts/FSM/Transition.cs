namespace BrendansProject
{
    /// <summary>
    /// Simple Class for determining the result of a decision.
    /// </summary>
    [System.Serializable] // Draw in inspector
    public class Transition
    {
        public Decision decision;
        public State trueState;
        public State falseState;
    }
}