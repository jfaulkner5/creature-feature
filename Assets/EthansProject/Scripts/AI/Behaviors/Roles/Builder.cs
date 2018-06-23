using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject { 

public class Builder : Villager
{

        public override HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

            goal.Add(new KeyValuePair<string, object>("expandStorage", (WorldInfo.filledStorage.Count > 0)));
            return goal;
        }
    }
}
