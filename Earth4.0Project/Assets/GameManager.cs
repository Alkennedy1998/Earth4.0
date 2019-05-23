using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

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
    public const float _COST_COTTON = 50.0f;

    public const float _POLLUTION_FACTORY_ONBUILD = 100.0f;
    public const float _POLLUTION_OTHER_ONBUILD = 15.0f;

    private const float _TICK_TIME = 4.0f; // There are 4 seconds between 'ticks'
    private const float _FAST_TICK_TIME = .1f;
    public const float _FACTORY_POLLUTION_PER_TICK = 2.0f;
    public const float _TREE_DEPOLLUTION_PER_TICK = 6.0f;
    public const float _FARM_FOOD_PER_TICK = 45.0f;
    public const float _FOOD_EATEN_PER_TICK = 6.0f;
    public const float _FACTORY_MONEY_PER_TICK = 8.0f;
    public const float _FACTORY_COTTON_PER_TICK = 6.0f;
    public const float _COTTON_PER_TICK = 9.0f;

    public const float _MAX_POLLUTION = 300.0f;
    public const float _MAX_SMOKE_PARTICLES = 50.0f;
    public const float _POLLUTION_TO_LARP = 1.0f;

    public const int _PEOPLE_PER_HOUSE = 5;

    #endregion

    #region Game Values

    public bool _debugEnabled;

    // Internal Values
    private float _currentTickTime = 0.0f;
    private float _fastTickTime = 0.0f;

    // Game Values
    public enum GameState { Playing, GameOver };
    public GameState _gameState;
    public GameObject _gameOverObject;

    public float _currentPollution;
    public float _currentMoney;
    public float _currentFood;
    public float _currentCotton;
    public int _currentFactoryWorkers, _currentFarmWorkers, _currentCottonWorkers;

    public float _cost_tree = 20.0f;
    private float _pollutionToAdd = 0.0f;

    // Game PreFabs
    public GameObject _personPrefab;
    public GameObject _factoryPrefab, _farmPrefab, _housePrefab, _treePrefab, _cottonPrefab;

    // Game Objects
    public List<GameObject> _personList = new List<GameObject>();
    public List<GameObject> _factoryList = new List<GameObject>();
    public List<GameObject> _farmList = new List<GameObject>();
    public List<GameObject> _houseList = new List<GameObject>();
    public List<GameObject> _treeList = new List<GameObject>();
    public List<GameObject> _cottonList = new List<GameObject>();
    public List<GameObject> _smogLayersList = new List<GameObject>();

    private TextMeshPro _moneyText, _foodText, _cottonText, _gameOverText;
    private TextMeshProUGUI _treeCostText;
    private Text _UIText;

    #endregion

    #region Initialization

    // Use this for initialization
    void Start()
    {
        _gameState = GameState.Playing;

        _currentPollution = _STARTING_POLLUTION;
        _currentMoney = _STARTING_MONEY;
        _currentFood = _STARTING_FOOD;
        _currentCotton = _STARTING_COTTON;

        _currentFactoryWorkers = 0;
        _currentFarmWorkers = 0;
        _currentCottonWorkers = 0;

        _UIText = GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>();
        _moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshPro>();
        _foodText = GameObject.Find("FoodText").GetComponent<TextMeshPro>();
        _cottonText = GameObject.Find("CottonText").GetComponent<TextMeshPro>();
        _treeCostText = GameObject.Find("ForestText").GetComponent<TextMeshProUGUI>();

        foreach (GameObject layer in _smogLayersList)
        {
            var rend = layer.GetComponent<Renderer>();
            rend.enabled = true;

            layer.transform.rotation = Random.rotation;
            setSmogLayerOpacity(layer, 1.0f);
        }
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        _currentTickTime += Time.deltaTime;
        _fastTickTime += Time.deltaTime;
        updateText();

        // If we have pollution to add (that's been queued up), add it 1 at a time
        if (_pollutionToAdd > 0.0f)
        {
            _currentPollution += Mathf.Clamp(_pollutionToAdd, 0.0f, _POLLUTION_TO_LARP);
            _pollutionToAdd -= Mathf.Clamp(_pollutionToAdd, 0.0f, _POLLUTION_TO_LARP);
        }

        // Update the smog layers
        for (int i = 0; i < _smogLayersList.Count; i++)
        {
            _smogLayersList[i].transform.Rotate(0.07f, 0.0f, 0.0f);
            setSmogLayerOpacity(_smogLayersList[i], Mathf.Clamp((_currentPollution / 100) - i, 0.0f, 1.0f));
        }

        if (_fastTickTime >= _FAST_TICK_TIME)
            onFastTick();

        if (_currentTickTime >= _TICK_TIME)
            onTick();
    }

    private void onFastTick()
    {
        _fastTickTime = 0.0f;

        _currentPollution += (_currentFactoryWorkers * _FACTORY_POLLUTION_PER_TICK - _treeList.Count * _TREE_DEPOLLUTION_PER_TICK) / (_TICK_TIME / _FAST_TICK_TIME);
        _currentPollution = Mathf.Clamp(_currentPollution, 0, _MAX_POLLUTION);
    }

    private void onTick()
    {
        _currentTickTime = 0f;

        if (_currentCotton > 0)
            _currentMoney += _currentFactoryWorkers * _FACTORY_MONEY_PER_TICK;

        _currentFood += _currentFarmWorkers * _FARM_FOOD_PER_TICK - _personList.Count * _FOOD_EATEN_PER_TICK;
        _currentCotton += _currentCottonWorkers * _COTTON_PER_TICK - _currentFactoryWorkers * _FACTORY_COTTON_PER_TICK;

        // Set to 0.0f if negative
        _currentFood = Mathf.Clamp(_currentFood, 0, 9999);
        _currentCotton = Mathf.Clamp(_currentCotton, 0, 9999);

        checkWinCondition();

        logValues();
    }

    private void updateText()
    {
        _moneyText.text = _currentMoney.ToString();
        _foodText.text = _currentFood.ToString();
        _cottonText.text = _currentCotton.ToString();
        _treeCostText.text = "$" + _cost_tree.ToString();
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
        _pollutionToAdd += _POLLUTION_FACTORY_ONBUILD;
        return true;
    }

    public bool addFarm(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _COST_FARM)
            return false;
        _currentMoney -= _COST_FARM;

        GameObject farm = instantiateOnWorld(_farmPrefab, location, rotation);
        _farmList.Add(farm);
        _pollutionToAdd += _POLLUTION_OTHER_ONBUILD;
        return true;
    }

    public bool addHouse(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _COST_HOUSE)
            return false;
        _currentMoney -= _COST_HOUSE;

        GameObject house = instantiateOnWorld(_housePrefab, location, rotation);
        _houseList.Add(house);
        _pollutionToAdd += _POLLUTION_OTHER_ONBUILD;

        // Add new people attached to this house
        for (int i = 0; i < _PEOPLE_PER_HOUSE; i++)
            addPerson(location, rotation, house);
        return true;
    }

    public bool addTree(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _cost_tree)
            return false;
        _currentMoney -= _cost_tree;
        _cost_tree += 10.0f;

        GameObject tree = instantiateOnWorld(_treePrefab, location, rotation);
        _treeList.Add(tree);
        return true;
    }

    public bool removeTree(GameObject tree)
    {
        _treeList.Remove(tree);
        Destroy(tree);
        return true;
    }

    public bool addCotton(Vector3 location, Quaternion rotation)
    {
        if (_currentMoney < _COST_COTTON)
            return false;
        _currentMoney -= _COST_COTTON;

        GameObject cotton = instantiateOnWorld(_cottonPrefab, location, rotation);
        _cottonList.Add(cotton);
        _pollutionToAdd += _POLLUTION_OTHER_ONBUILD;
        return true;
    }

    public bool addPerson(Vector3 location, Quaternion rotation, GameObject house)
    {
        GameObject person = instantiateOnWorld(_personPrefab, location, rotation);
        person.GetComponent<PersonController>()._attachedHouse = house;
        _personList.Add(person);
        return true;
    }

    #endregion

    #region WinCondition

    private void checkWinCondition()
    {
        if (_currentPollution > _GAME_LOSE_POLLUTION || _currentFood <= 0.0f)
        {
            _gameOverObject.SetActive(true);
            _gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshPro>();
            _gameOverText.text = "GAME OVER!";
            _UIText.text = "";
            _gameState = GameState.GameOver;
            Debug.Log("GAME OVER!");
        }
        else if (_currentMoney > _GAME_WIN_MONEY)
        {
            _gameOverObject.SetActive(true);
            _gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshPro>();
            _gameOverText.text = "YOU WIN!";
            _UIText.text = "";
            _gameState = GameState.GameOver;
            Debug.Log("YOU WIN!");
        }
    }

    #endregion

    #region UtilityFunctions

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
        if (_debugEnabled)
        {
            Debug.Log("Pollution: " + _currentPollution + "     Money: " + _currentMoney + "     Food: " + _currentFood + "     Cotton: " + _currentCotton);
            //Debug.Log("FactoryWorkers: " + _currentFactoryWorkers + "     FarmWorkers: " + _currentFarmWorkers + "     CottonWorkers: " + _currentCottonWorkers);
        }
    }

    #endregion
}
