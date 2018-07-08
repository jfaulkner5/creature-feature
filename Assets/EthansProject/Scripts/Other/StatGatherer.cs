using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EthansProject {
    public class StatGatherer : MonoBehaviour {

        private void OnMouseDown()
        {         
            UIStatDisplay.instance.SetText(gameObject.name, GetComponent<Villager>().currentHungerLevel.ToString(), GetComponent<AgentStorage>().resourceHolding.ToString(), GetComponent<Villager>().role.ToString());
        }
    }
}
