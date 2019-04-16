using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerRay : MonoBehaviour {

    private LineRenderer lineRenderer;
    public int lengthOfLineRenderer =200;
    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;
    }
	
	// Update is called once per frame
	void Update () {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.forward);
        Debug.Log(transform.forward);
        //lineRenderer.SetPosition(2, new Vector3 (0,0,0));
    }
}
