using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{

    public float berryRegrowthRate;
    float currentRate;

    public int InitBerryCount = 10;
    public int currentBerryCount;
    public float randomnessPercent = 10;

    public bool hassBerries = true;
    public bool debug_takeBerries;

    GameObject berries
    {
        get { return transform.GetChild(0).gameObject; }
    }
    // Use this for initialization
    void Start()
    {
        currentBerryCount = InitBerryCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (debug_takeBerries)
        {
            debug_takeBerries = false;
            TakeBerries();
        }

        if (currentRate > 0)
            {
                currentRate -= Time.deltaTime;
            }
            else
            {
                if (!hassBerries)
                    BerriesRegrew();
            }
    }

    void BerriesRegrew()
    {
        int randChance = (int) Random.Range(-randomnessPercent, randomnessPercent);
        float newBerryCount = InitBerryCount * randChance;

        currentBerryCount = InitBerryCount + Mathf.RoundToInt(randChance);
        hassBerries = true;

        berries.SetActive(true);
      //  print(currentBerryCount + ", " + randChance);
    }

    public int TakeBerries()
    {
        hassBerries = false;
        currentRate = berryRegrowthRate;

        berries.SetActive(false);

        return currentBerryCount;
    }

}
