using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Constants
    private const float _tickTime = 5.0f; // There are 5 seconds between 'ticks'
    public const float _factoryPollutionPerTick = 1.0f;
    public const float _farmFoodPerTick = 1.0f;

    // Internal Values
    private float _currentTickTime = 0.0f;

    // Game Values
    public float _currentPollution = 0.0f;
    public float _currentMoney = 0.0f;
    public float _currentFood = 0.0f;
    public float _currentCotton = 0.0f;

    // Game Objects
    public List<GameObject> _factoryList = new List<GameObject>();
    public List<GameObject> _farmList = new List<GameObject>();
    public List<GameObject> _houseList = new List<GameObject>();
    public List<GameObject> _treeList = new List<GameObject>();

    private GameObject _moneyText, _foodText, _cottonText;

	// Use this for initialization
	void Start () {
        _moneyText = GameObject.Find("MoneyText");
        _foodText = GameObject.Find("FoodText");
        _cottonText = GameObject.Find("CottonText");
    }
	
	// Update is called once per frame
	void Update () {

        _currentTickTime += Time.deltaTime;

        if (_currentTickTime >= _tickTime)
            onTick();
        
	}

    private void onTick()
    {
        _currentTickTime = 0f;

        _currentPollution += _factoryList.Count * _factoryPollutionPerTick;
        _currentFood += _farmList.Count * _farmFoodPerTick;

        updateText();

        logValues();
    }

    private void updateText()
    {
        _moneyText.GetComponent<TextMeshPro>().text = _currentMoney.ToString();
        _foodText.GetComponent<TextMeshPro>().text = _currentFood.ToString();
        _cottonText.GetComponent<TextMeshPro>().text = _currentCotton.ToString();
    }

    public void addFactory(GameObject factory) { _factoryList.Add(factory); }
    public void addFarm(GameObject farm) { _farmList.Add(farm); }
    public void addHouse(GameObject house) { _houseList.Add(house); }
    public void addTree(GameObject tree) { _treeList.Add(tree); }

    private void logValues()
    {
        Debug.Log("Pollution: " + _currentPollution + "     Money: " + _currentMoney + "     Food: " + _currentFood + "     Cotton: " + _currentCotton);
    }
}
