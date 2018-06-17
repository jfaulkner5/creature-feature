using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
