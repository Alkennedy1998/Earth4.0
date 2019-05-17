using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIray : MonoBehaviour {

	public LineRenderer _ray;
	
	public GameObject _marker;

	public float maximumLength = 10;
	// Use this for initialization
	void Start () {
		_ray = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
		_ray.SetPosition(0, transform.position);
		_ray.SetPosition(1,_marker.transform.position);
	}
}
