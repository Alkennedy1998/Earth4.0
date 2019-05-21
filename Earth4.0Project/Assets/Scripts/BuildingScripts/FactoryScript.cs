using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : _WorkableBuildingScript {

    private const int _MAX_FACTORY_WORKERS = 3;
    private const float _TICK_TIME = 5.0f; // There are 5 seconds between 'ticks'
	private const float _SMOKE_OFFSET_MULTIPLIER = 1.5f; // Height above factory

    // Internal Values
    private float _currentTickTime = 0.0f;

	private GameObject _world;
	public GameObject _smokePrefab;

	// Use this for initialization
	new void Start () {
		base.Start();  // Run BaseClass' Start() method
		_world = GameObject.Find("World");
	}

	// Update is called once per frame
	void Update () {
        _currentTickTime += Time.deltaTime;

        if (_currentTickTime >= _TICK_TIME)
            onTick();
	}

	void onTick() {
		_currentTickTime = 0f;

		Vector3 factoryPositionFromCenter = transform.position - _world.transform.position;
		Vector3 smokePositionFromCenter = _SMOKE_OFFSET_MULTIPLIER * factoryPositionFromCenter;
		Vector3 smokeLocation = smokePositionFromCenter + _world.transform.position;

		GameObject smoke = Instantiate(_smokePrefab, smokeLocation, transform.rotation);
		//smoke.transform.parent = transform;
	}

    public override int getMaxWorkers() { return _MAX_FACTORY_WORKERS; }
}
