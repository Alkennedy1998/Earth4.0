using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Constants

    // Constants
    public const float _STARTING_MONEY = 400.0f;
    public const float _STARTING_FOOD = 200.0f;
    public const float _STARTING_COTTON = 200.0f;
    private const float _STARTING_POLLUTION = 0.0f;

    private const float _GAME_WIN_MONEY = 2000.0f;
    private const float _GAME_LOSE_POLLUTION = 300.0f;

    public const float _COST_FACTORY = 100.0f;
    public const float _COST_FARM = 50.0f;
    public const float _COST_HOUSE = 50.0f;
    public const float _COST_TREE = 20.0f;
    public const float _COST_COTTON = 50.0f;

    private const float _TICK_TIME = 4.0f; // There are 4 seconds between 'ticks'
    public const float _FACTORY_POLLUTION_PER_TICK = 2.0f;
    public const float _TREE_DEPOLLUTION_PER_TICK = 6.0f;
    public const float _FARM_FOOD_PER_TICK = 45.0f;
    public const float _FOOD_EATEN_PER_TICK = 6.0f;
    public const float _FACTORY_MONEY_PER_TICK = 8.0f;
    public const float _FACTORY_COTTON_PER_TICK = 6.0f;
    public const float _COTTON_PER_TICK = 9.0f;

    public const float _MAX_POLLUTION = 30.0f;

    public const int _PEOPLE_PER_HOUSE = 5;

    #endregion

    #region Game Values

    // Internal Values
    private float _currentTickTime = 0.0f;

    // Game Values
    public float _currentPollution;
    public float _currentMoney;
    public float _currentFood;
    public float _currentCotton;

    public int _currentFactoryWorkers, _currentFarmWorkers, _currentCottonWorkers;

    // Game PreFabs
    public GameObject _personPrefab;
    public GameObject _factoryPrefab, _farmPrefab, _housePrefab, _treePrefab, _cottonPrefab, _smogLayerPrefab;

    // Game Objects
    public List<GameObject> _personList = new List<GameObject>();
    public List<GameObject> _factoryList = new List<GameObject>();
    public List<GameObject> _farmList = new List<GameObject>();
    public List<GameObject> _houseList = new List<GameObject>();
    public List<GameObject> _treeList = new List<GameObject>();
    public List<GameObject> _cottonList = new List<GameObject>();
    public List<GameObject> _smogLayersList = new List<GameObject>();

    private GameObject _moneyText, _foodText, _cottonText;

    #endregion

    #region Initialization

    // Use this for initialization
    void Start () {
        _currentPollution = _STARTING_POLLUTION;
        _currentMoney = _STARTING_MONEY;
        _currentFood = _STARTING_FOOD;
        _currentCotton = _STARTING_COTTON;

        _currentFactoryWorkers = 0;
        _currentFarmWorkers = 0;
        _currentCottonWorkers = 0;

        _moneyText = GameObject.Find("MoneyText");
        _foodText = GameObject.Find("FoodText");
        _cottonText = GameObject.Find("CottonText");

        foreach (GameObject layer in _smogLayersList)
        {
            layer.transform.rotation = Random.rotation;
            setSmogLayerOpacity(layer, 1.0f);
        }
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update () {

        _currentTickTime += Time.deltaTime;
        updateText();
        updateSmog();

        if (_currentTickTime >= _TICK_TIME)
            onTick();
	}

    private void onTick()
    {
        _currentTickTime = 0f;
        
        _currentMoney += _currentFactoryWorkers * _FACTORY_MONEY_PER_TICK;
        _currentFood += _currentFarmWorkers * _FARM_FOOD_PER_TICK - _personList.Count * _FOOD_EATEN_PER_TICK;
        _currentCotton += _currentCottonWorkers * _COTTON_PER_TICK - _currentFactoryWorkers * _FACTORY_COTTON_PER_TICK;

        // Set to 0.0f if negative
        _currentFood = clampAtZero(_currentFood);
        _currentCotton = clampAtZero(_currentCotton);

        checkWinCondition();

        logValues();
    }

    private void updateText()
    {
        _moneyText.GetComponent<TextMeshPro>().text = _currentMoney.ToString();
        _foodText.GetComponent<TextMeshPro>().text = _currentFood.ToString();
        _cottonText.GetComponent<TextMeshPro>().text = _currentCotton.ToString();
    }

    private void updatePollution()
    {
        _fastTickTime = 0.0f;

        _currentPollution += (_currentFactoryWorkers * _FACTORY_POLLUTION_PER_TICK - _treeList.Count * _TREE_DEPOLLUTION_PER_TICK) / (_TICK_TIME / _FAST_TICK_TIME);
        _currentPollution = clampAtZero(_currentPollution);
    }

    #endregion

    #region BuildingPlacement

    public GameObject instantiateOnWorld(GameObject prefab, Vector3 location, Quaternion rotation)
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

        // Add new people attached to this house
        for (int i = 0; i < _PEOPLE_PER_HOUSE; i++) {
            GameObject person = instantiateOnWorld(_personPrefab, location, rotation);
            person.GetComponent<PersonController>()._attachedHouse = house;
            _personList.Add(person);
        }
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

    public bool addCotton(Vector3 location, Quaternion rotation) {
        if (_currentMoney < _COST_COTTON)
            return false;
        _currentMoney -= _COST_COTTON;

        GameObject cotton = instantiateOnWorld(_cottonPrefab, location, rotation);
        _cottonList.Add(cotton);
        return true;
    }

    #endregion

    #region WinCondition

    private void checkWinCondition()
    {
        if (_currentPollution > _GAME_LOSE_POLLUTION || _currentFood <= 0.0f) {
            Debug.Log("GAME OVER!");
            GameObject text = GameObject.Find("GameOverText");
            text.GetComponent<Renderer>().enabled = true;


        } else if (_currentMoney > _GAME_WIN_MONEY) {
            Debug.Log("YOU WIN!");
            GameObject text = GameObject.Find("WinGameText");
            text.GetComponent<Renderer>().enabled = true;
        }
    }

    #endregion

    #region UtilityFunctions

    private float clampAtZero(float val) {
        return val < 0.0f ? 0.0f : val;
    }

    private void setSmogLayerOpacity(GameObject layer, float value)
    {
        Renderer rend = layer.GetComponent<Renderer>();
        Color result = rend.material.color;
        result.a = value;
        rend.material.color = result;
    }

    #endregion

    #region Debugging

    private void logValues()
    {
        //Debug.Log("Pollution: " + _currentPollution + "     Money: " + _currentMoney + "     Food: " + _currentFood + "     Cotton: " + _currentCotton);
        //Debug.Log("FactoryWorkers: " + _currentFactoryWorkers + "     FarmWorkers: " + _currentFarmWorkers + "     CottonWorkers: " + _currentCottonWorkers);
    }

    #endregion
}
