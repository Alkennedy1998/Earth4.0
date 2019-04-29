using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayCaseRotator : MonoBehaviour {

	private float rotateSpeed = 5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
	}
}
