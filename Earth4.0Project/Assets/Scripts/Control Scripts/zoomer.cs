using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomer : MonoBehaviour {

    public GameObject _gameOverText;
    GameObject _AStar;
    Pathfinding.NavMeshGraph graph;
    float MAX_ZOOM_X = -1.475525f;

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

        if (forward_distance != 0.0f)
        {
            if (transform.position.x <= MAX_ZOOM_X || forward_distance < 0.0f)
            {
                transform.Translate(Vector3.right * forward_distance, Space.World);
                _gameOverText.transform.Translate(Vector3.right * forward_distance, Space.World);
                graph.offset += Vector3.right * forward_distance;
                _AStar.GetComponent<AstarPath>().Scan();
            }
        }
    }
}
