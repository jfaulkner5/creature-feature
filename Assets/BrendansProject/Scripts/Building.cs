using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendansProject
{

    public class Building : Unit
    {

        public int humansInside;

        //public GameObject humanPrefab;

        private void OnDisable()
        {

            for (int i = 0; i <= humansInside; i++)
            {

                GameObject go = Instantiate(ProcGenerator.instance.humans, targetPos, Quaternion.identity, ProcGenerator.instance.transform);
                ProcGenerator.instance.humansList.Add(go.transform); // add to target list for later reference

            }

        }


    }
}