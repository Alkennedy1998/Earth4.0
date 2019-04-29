using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Constants

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

    #endregion

    #region Game Values

    // Internal Values
    private float _currentTickTime = 0.0f;

    // Game Values
    public float _currentPollution;
    public float _currentMoney;
    public float _currentFood;
    public float _currentCotton;

    // Game PreFabs
    public GameObject _personPrefab;
    public GameObject _factoryPrefab, _farmPrefab, _housePrefab, _treePrefab;

    // Game Objects
    public List<GameObject> _personList = new List<GameObject>();
    public List<GameObject> _factoryList = new List<GameObject>();
    public List<GameObject> _farmList = new List<GameObject>();
    public List<GameObject> _houseList = new List<GameObject>();
    public List<GameObject> _treeList = new List<GameObject>();

    private GameObject _moneyText, _foodText, _cottonText;

    #endregion

    #region Initialization

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

    #endregion

    #region Update

    // Update is called once per frame
    void Update () {

        _currentTickTime += Time.deltaTime;
        updateText();

        if (_currentTickTime >= _TICK_TIME)
            onTick();
	}

    private void onTick()
    {
        _currentTickTime = 0f;

        _currentPollution += _factoryList.Count * _FACTORY_POLLUTION_PER_TICK;
        _currentMoney += _factoryList.Count * _FACTORY_MONEY_PER_TICK;
        _currentFood += _farmList.Count * _FARM_FOOD_PER_TICK;

        logValues();
    }

    private void updateText()
    {
        _moneyText.GetComponent<TextMeshPro>().text = _currentMoney.ToString();
        _foodText.GetComponent<TextMeshPro>().text = _currentFood.ToString();
        _cottonText.GetComponent<TextMeshPro>().text = _currentCotton.ToString();
    }

    #endregion

    #region BuildingPlacement

    private GameObject instantiateOnWorld(GameObject prefab, Vector3 location, Quaternion rotation)
    {
        GameObject newItem = Instantiate(prefab, location, rotation);
        newItem.transform.Rotate(90, 0, 0);
        newItem.transform.parent = transform;
        return newItem;
    }

    public bool addFactory(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _COST_FACTORY)
            return false;
        _currentMoney -= _COST_FACTORY;

        GameObject factory = instantiateOnWorld(_factoryPrefab, location, rotation);
        _factoryList.Add(factory);
        return true;
    }

    public bool addFarm(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _COST_FARM)
            return false;
        _currentMoney -= _COST_FARM;

        GameObject farm = instantiateOnWorld(_farmPrefab, location, rotation);
        _farmList.Add(farm);
        return true;
    }

    public bool addHouse(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _COST_HOUSE)
            return false;
        _currentMoney -= _COST_HOUSE;

        GameObject house = instantiateOnWorld(_housePrefab, location, rotation);
        _houseList.Add(house);

        GameObject person = instantiateOnWorld(_personPrefab, location, rotation);
        person.GetComponent<PersonController>()._attachedHouse = house;
        _personList.Add(person);
        return true;
    }

    public bool addTree(Vector3 location, Quaternion rotation) {
        if (_currentMoney < _COST_TREE)
            return false;
        _currentMoney -= _COST_TREE;

        GameObject tree = instantiateOnWorld(_treePrefab, location, rotation);
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
