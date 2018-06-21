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

        
        public override HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

            goal.Add(new KeyValuePair<string, object>("collectResource", true));
            return goal;
        }
    }
}
