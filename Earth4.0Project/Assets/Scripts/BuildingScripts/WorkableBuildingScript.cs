using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _WorkableBuildingScript : MonoBehaviour {

	private const int MAX_WORKERS = 5;

	public int currentWorkers;

	// Use this for initialization
	public void Start () {
		currentWorkers = 0;
	}

	// Update is called once per frame
	void Update () {

	}

	public bool isFull() {
		return currentWorkers >= MAX_WORKERS;
	}

	public bool isEmpty() {
		return currentWorkers <= 0;
	}

	public bool addWorker() {
		if (isFull())
			return false;
		currentWorkers++;
		return true;
	}

	public bool removeWorker() {
		if (isEmpty())
			return false;
		currentWorkers--;
		return true;
	}
}
