using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : _WorkableBuildingScript {

	private const int _MAX_HOUSE_POPULATION = 5;

	// Use this for initialization
	new void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override int getMaxWorkers() { return _MAX_HOUSE_POPULATION; }
}
