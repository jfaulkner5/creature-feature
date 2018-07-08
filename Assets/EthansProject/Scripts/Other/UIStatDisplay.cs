using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace EthansProject
{
    public class UIStatDisplay : MonoBehaviour
    {

        public static UIStatDisplay instance;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);

            }
            else
            {
                instance = this;
            }
        }

        public Text agentName, hunger, resourceCount, agentRole;

        public void SetText(string info1, string info2, string info3, string info4)
        {
            agentName.text = info1;
            hunger.text = info2;
            resourceCount.text = info3;
            agentRole.text = info4;
        }

    }
}
