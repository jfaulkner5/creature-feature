using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject { 

public class Builder : Villager
{

        public override HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            goal.Clear();

            if (needFood)
                goal.Add(new KeyValuePair<string, object>("dontDieOfHunger", needFood));
            else
                goal.Add(new KeyValuePair<string, object>("expandStorage", ((WorldInfo.filledStorage.Count > 0))));
            return goal;
        }
    }
}
