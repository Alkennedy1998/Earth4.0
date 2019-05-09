using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		zoomForwardandBack();
	}
	 void zoomForwardandBack()
    {
        Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float forward_distance = Time.deltaTime * rightStick.y * 3f;
        transform.Translate(Vector3.right*forward_distance,Space.World);

		Debug.Log("rightStick: "+ rightStick.x);
    }
}
