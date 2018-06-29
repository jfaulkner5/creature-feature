using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject
{
    /// <summary>
    /// Inherates from 
    /// </summary>
    public class Gatherer : Villager
    {

        public void GetActions()
        {
            GetComponent<GatherResourceAction>().SwitchRoles();
            GetComponent<StoreResourceAction>().SwitchRoles();
        }

        
        public override HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            goal.Clear();
            
            if (needFood)
                goal.Add(new KeyValuePair<string, object>("dontDieOfHunger", needFood));
            else
                goal.Add(new KeyValuePair<string, object>("collectResource", (!needFood)));

            return goal;
        }
    }
}
