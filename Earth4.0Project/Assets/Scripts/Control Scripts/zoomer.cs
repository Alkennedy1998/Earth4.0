using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomer : MonoBehaviour {
    GameObject _AStar;
    Pathfinding.NavMeshGraph graph;

    // Use this for initialization
    void Start ()
    {
        _AStar = GameObject.Find("A*");
        graph = _AStar.GetComponent<AstarPath>().graphs[0] as Pathfinding.NavMeshGraph;
    }
	
	// Update is called once per frame
	void Update () {
		zoomForwardandBack();
	}

	 void zoomForwardandBack()
     {
         Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
         float forward_distance = Time.deltaTime * rightStick.y * 3f;
         transform.Translate(Vector3.right * forward_distance, Space.World);

         if (forward_distance != 0.0f)
         {
             graph.offset += Vector3.right * forward_distance;
             _AStar.GetComponent<AstarPath>().Scan();
         }
    }
}
