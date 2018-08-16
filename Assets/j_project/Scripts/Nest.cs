using jfaulkner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nest : MonoBehaviour
{
    public static Nest instance;

    [Header("Nest and spawner prefabs")]
    public GameObject antPrefab;
    [HideInInspector] public GameObject antNest;

    public int antSpawnLimit;
    private int antCurrentSpawned = 0;

    [Header("Resources")]
    public int sugaryFood = 0;
    public int meatFood = 0;
    public int meatValue = 20;
    public int sugarValue = 20;

    [SerializeField] private int maxFood = 100;

    [HideInInspector] public List<GameObject> candyList;
    [HideInInspector] public List<GameObject> meatList;

    [HideInInspector]
    public enum FoodType
    {
        SugaryFood,
        FattyFood
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }
    // Use this for initialization
    void Start()
    {
        candyList = new List<GameObject>();
        meatList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnSequencer());
    }

    public IEnumerator SpawnSequencer()
    {
        Debug.Log("Spawn was called");

        while (antCurrentSpawned < antSpawnLimit)
        {
            antCurrentSpawned++;
            GameObject ant = Instantiate(antPrefab, this.transform);
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return null;
    }

    public void FoodCollection(FoodType foodType)
    {
        switch (foodType)
        {
            case FoodType.SugaryFood:
                break;
            case FoodType.FattyFood:
                break;
            default:
                break;
        }
    }

    public FoodType ReturnFoodType()
    {
        if (sugaryFood <= meatFood)
        {
            return FoodType.SugaryFood;
        }
        else if (sugaryFood > meatFood)
        {
            return FoodType.FattyFood;
        }

        throw new System.Exception();
    }

    public Vector3 GiveOrder()
    {
        if (ReturnFoodType() == FoodType.SugaryFood)
        {
            int randIndex = Random.Range(0, candyList.Count - 1);
            return candyList[randIndex].transform.position;
        }
        else
        {
            int randIndex = Random.Range(0, meatList.Count - 1);
            return meatList[randIndex].transform.position;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ant"))
        {
            BasicAnt currentAnt = other.gameObject.GetComponent<BasicAnt>();

            if (currentAnt.hasFood && currentAnt.foodType == FoodType.SugaryFood)
            {
                currentAnt.hasFood = false;
                sugaryFood += sugarValue;
                if (sugaryFood > maxFood)
                    sugaryFood = maxFood;
            }

            if(currentAnt.hasFood && currentAnt.foodType == FoodType.FattyFood)
            {
                currentAnt.hasFood = false;
                meatFood += meatValue;
                if (meatFood > maxFood)
                    meatFood = maxFood;
            }


        }
    }
}
