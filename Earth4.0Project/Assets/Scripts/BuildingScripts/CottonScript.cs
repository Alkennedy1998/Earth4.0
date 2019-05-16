using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CottonScript : _WorkableBuildingScript {

    private const int _MAX_COTTON_WORKERS = 2;

	// Use this for initialization
	new void Start () {
		base.Start();
	}

	// Update is called once per frame
	void Update () {

	}

    public override int getMaxWorkers() { return _MAX_COTTON_WORKERS; }
}
