using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Constants
    const float tickTime = 5.0f; // There are 5 seconds between 'ticks'
    const float factoryPollutionPerTick = 1.0f;

    // Internal Values
    private float currentTickTime = 0f;

    // Game Values
    public float currentPollution = 0.0f;
    public float currentMoney = 0.0f;
    public float currentFood = 0.0f;
    public float currentCotton = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        currentTickTime += Time.deltaTime;

        if (currentTickTime >= tickTime)
            onTick();


	}

    private void onTick()
    {
        currentTickTime = 0f;

        int numFactories = GameObject.FindGameObjectsWithTag("Factory").Length;
        currentPollution += numFactories * factoryPollutionPerTick;

        logValues();
    }

    private void logValues()
    {
        Debug.Log("Pollution: " + currentPollution + "     Money: " + currentMoney + "     Food: " + currentFood + "     Cotton: " + currentCotton);
    }
}
