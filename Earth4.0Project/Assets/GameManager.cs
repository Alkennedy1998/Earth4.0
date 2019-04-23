﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Constants
    private const float tickTime = 5.0f; // There are 5 seconds between 'ticks'
    public const float factoryPollutionPerTick = 1.0f;
    public const float farmFoodPerTick = 1.0f;

    // Internal Values
    private float currentTickTime = 0.0f;

    // Game Values
    public float currentPollution = 0.0f;
    public float currentMoney = 0.0f;
    public float currentFood = 0.0f;
    public float currentCotton = 0.0f;

    // Game Objects
    public List<GameObject> factoryList = new List<GameObject>();
    public List<GameObject> farmList = new List<GameObject>();
    public List<GameObject> houseList = new List<GameObject>();
    public List<GameObject> treeList = new List<GameObject>();

    private GameObject moneyText, foodText, cottonText;

	// Use this for initialization
	void Start () {
        moneyText = GameObject.Find("MoneyText");
        foodText = GameObject.Find("FoodText");
        cottonText = GameObject.Find("CottonText");
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

        currentPollution += factoryList.Count * factoryPollutionPerTick;
        currentFood += farmList.Count * farmFoodPerTick;

        updateText();

        logValues();
    }

    private void updateText()
    {
        moneyText.GetComponent<TextMeshPro>().text = currentMoney.ToString();
        foodText.GetComponent<TextMeshPro>().text = currentFood.ToString();
        cottonText.GetComponent<TextMeshPro>().text = currentCotton.ToString();
    }

    public void addFactory(GameObject factory) { factoryList.Add(factory); }
    public void addFarm(GameObject farm) { farmList.Add(farm); }
    public void addHouse(GameObject house) { houseList.Add(house); }
    public void addTree(GameObject tree) { treeList.Add(tree); }

    private void logValues()
    {
        Debug.Log("Pollution: " + currentPollution + "     Money: " + currentMoney + "     Food: " + currentFood + "     Cotton: " + currentCotton);
    }
}
