using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotater : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Debug.Log(leftStick);
        float leftRotate = Time.deltaTime * leftStick.x * 90;
        float rightRotate = Time.deltaTime * leftStick.y * 90;
        transform.Rotate(0, leftRotate, rightRotate, Space.World);
    }
}
