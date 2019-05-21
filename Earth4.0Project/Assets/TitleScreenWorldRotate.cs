using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenWorldRotate : MonoBehaviour {

    private float rotateSpeed = -5f;
    private const float _WAIT_TIME = .1f;
    private float _currentWaitTime = 0.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        float leftRotate = Time.deltaTime * leftStick.x * -30;
        float rightRotate = Time.deltaTime * leftStick.y * 30;
        _currentWaitTime += Time.deltaTime;

        if (leftRotate != 0 || rightRotate != 0.0f)
        {
            transform.Rotate(0, leftRotate, rightRotate, Space.World);
            _currentWaitTime = 0.0f;
        }
        else if (_currentWaitTime >= _WAIT_TIME)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
