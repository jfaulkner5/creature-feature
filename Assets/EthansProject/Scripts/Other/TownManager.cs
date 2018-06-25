using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour {


    public GameObject berryGatherersPrefab, woodGathererPrefab, builderPrefab; // same VV

    [Header("Spawn Count")]
    public int berryGatherers, woodGatherers, builders; //TODO: add builder and warter etc villagers when the work lol



	// Use this for initialization
	void Start () {

        for (int i = 0; i < berryGatherers; i++)
        {
            Instantiate(berryGatherersPrefab, transform.position, Quaternion.identity);
        }

        for (int i = 0; i < woodGatherers; i++)
        {
            Instantiate(woodGathererPrefab, transform.position, Quaternion.identity);
        }

        for (int i = 0; i < builders; i++)
        {
            Instantiate(builderPrefab, transform.position, Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
