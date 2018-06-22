using System.Collections.Generic;

/// <summary>
/// Holds all the core methods for the abstract Roles(Villager.cs) to inherit from.
/// </summary>
public interface IGOAP
{

    HashSet<KeyValuePair<string, object>> GetWorldState();

    HashSet<KeyValuePair<string, object>> CreateGoalState();

    void PlanFailed(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions);

    void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions);

    void ActionFinished();

    void PlanAborted(GOAPAction aborter);

    bool MoveAgent(GOAPAction nextAction);

}
