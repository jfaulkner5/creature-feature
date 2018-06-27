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
            goal.Add(new KeyValuePair<string, object>("collectResource", true));
            goal.Add(new KeyValuePair<string, object>("stayFull", true));

            return goal;
        }
    }
}
