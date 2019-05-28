using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : _WorkableBuildingScript {

    private const int _MAX_FACTORY_WORKERS = 3;

	private GameObject _world;
    private ParticleSystem _smoke;

	// Use this for initialization
	new void Start () {
		base.Start();  // Run BaseClass' Start() method
		_world = GameObject.Find("World");
        _smoke = this.GetComponentsInChildren<ParticleSystem>()[0];
	}

	// Update is called once per frame
	void Update () {
        var emission = _smoke.emission;
        if (_currentWorkers > 0)
            emission.enabled = true;
        else
            emission.enabled = false;
	}

    public override int getMaxWorkers() { return _MAX_FACTORY_WORKERS; }
}
