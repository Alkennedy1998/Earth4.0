using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Constants
    public const float _STARTING_MONEY = 3000.0f;
    public const float _STARTING_FOOD = 1000.0f;
    public const float _STARTING_COTTON = 1000.0f;
    private const float _STARTING_POLLUTION = 0.0f;

    public const float _COST_FACTORY = 100.0f;
    public const float _COST_FARM = 50.0f;
    public const float _COST_HOUSE = 50.0f;
    public const float _COST_TREE = 20.0f;

    private const float _TICK_TIME = 5.0f; // There are 5 seconds between 'ticks'
    public const float _FACTORY_POLLUTION_PER_TICK = 1.0f;
    public const float _FARM_FOOD_PER_TICK = 1.0f;
    public const float _FACTORY_MONEY_PER_TICK = 2.0f;

    // Internal Values
    private float _currentTickTime = 0.0f;

    // Game Values
    public float _currentPollution;
    public float _currentMoney;
    public float _currentFood;
    public float _currentCotton;

    // Game Objects
    public List<GameObject> _factoryList = new List<GameObject>();
    public List<GameObject> _farmList = new List<GameObject>();
    public List<GameObject> _houseList = new List<GameObject>();
    public List<GameObject> _treeList = new List<GameObject>();

    private GameObject _moneyText, _foodText, _cottonText;

	// Use this for initialization
	void Start () {
        _currentPollution = _STARTING_POLLUTION;
        _currentMoney = _STARTING_MONEY;
        _currentFood = _STARTING_FOOD;
        _currentCotton = _STARTING_COTTON;

        _moneyText = GameObject.Find("MoneyText");
        _foodText = GameObject.Find("FoodText");
        _cottonText = GameObject.Find("CottonText");
    }
	
	// Update is called once per frame
	void Update () {

        _currentTickTime += Time.deltaTime;

        if (_currentTickTime >= _TICK_TIME)
            onTick();
        
	}

    private void onTick()
    {
        _currentTickTime = 0f;

        _currentPollution += _factoryList.Count * _FACTORY_POLLUTION_PER_TICK;
        _currentMoney += _factoryList.Count * _FACTORY_MONEY_PER_TICK;
        _currentFood += _farmList.Count * _FARM_FOOD_PER_TICK;

        updateText();

        logValues();
    }

    private void updateText()
    {
        _moneyText.GetComponent<TextMeshPro>().text = _currentMoney.ToString();
        _foodText.GetComponent<TextMeshPro>().text = _currentFood.ToString();
        _cottonText.GetComponent<TextMeshPro>().text = _currentCotton.ToString();
    }

    #region BuildingPlacement

    public bool addFactory(GameObject factory)
    {
        if (_currentMoney < _COST_FACTORY)
            return false;

        _currentMoney -= _COST_FACTORY;
        _factoryList.Add(factory);
        return true;
    }

    public bool addFarm(GameObject farm)
    {
        if (_currentMoney < _COST_FARM)
            return false;

        _currentMoney -= _COST_FARM;
        _farmList.Add(farm);
        return true;
    }

    public bool addHouse(GameObject house)
    {
        if (_currentMoney < _COST_HOUSE)
            return false;

        _currentMoney -= _COST_HOUSE;
        _houseList.Add(house);
        return true;
    }

    public bool addTree(GameObject tree) {
        if (_currentMoney < _COST_TREE)
            return false;

        _currentMoney -= _COST_TREE;
        _treeList.Add(tree);
        return true;
    }

    #endregion

    #region Debugging

    private void logValues()
    {
        Debug.Log("Pollution: " + _currentPollution + "     Money: " + _currentMoney + "     Food: " + _currentFood + "     Cotton: " + _currentCotton);
    }

    #endregion
}
