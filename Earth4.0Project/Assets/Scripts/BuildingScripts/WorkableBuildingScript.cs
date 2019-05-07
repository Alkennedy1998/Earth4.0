using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _WorkableBuildingScript : MonoBehaviour {

	private const int _MAX_WORKERS = 5;

	public bool _isFull;
	public bool _isEmpty;
	public int _currentWorkers;

	// Use this for initialization
	public void Start () {
		_currentWorkers = 0;
		_isFull = false;
		_isEmpty = true;
	}

	// Update is called once per frame
	void Update () {

	}

	public bool addWorker() {
		if (_isFull)
			return false;

		_currentWorkers++;
		if (_currentWorkers >= _MAX_WORKERS)
			_isFull = true;
		if (_isEmpty)
			_isEmpty = false;
		return true;
	}

	public bool removeWorker() {
		if (_isEmpty)
			return false;

		_currentWorkers--;
		if (_currentWorkers <= 0)
			_isEmpty = true;
		if (_isFull)
			_isFull = false;
		return true;
	}
}
