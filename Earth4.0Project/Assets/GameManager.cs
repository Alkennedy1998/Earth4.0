using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Game Values;
    public float currentPollution = 0.0f;

    // Constants
    const float factoryPollutionPerSecond = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        int numFactories = GameObject.FindGameObjectsWithTag("Factory").Length;
        currentPollution += numFactories * factoryPollutionPerSecond * Time.deltaTime;

        Debug.Log(currentPollution);

	}
}
