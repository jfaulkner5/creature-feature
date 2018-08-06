using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Tooltip("The in-game time in 24 hour time"), Header("In Game Time")]
    public int time = 0000;

    public Transform directionalLight;

    public float timerRate, daySpeed = 1;
    protected Quaternion directionRotation;
	// Use this for initialization
	void Start () {
        StartCoroutine(Timer());
	}
	public IEnumerator Timer()
    {
        yield return new WaitForSeconds(timerRate);
        TickTime();
    }
	// Update is called once per frame
	void Update () {
        directionRotation.x += Time.deltaTime * daySpeed;
        directionalLight.rotation = directionRotation;
	}

    public void TickTime()
    {

    }
}
